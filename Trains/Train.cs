using Godot;
using System;
using System.Collections.Generic;

public partial class Train : PathFollow2D
{
    public int lapsComplete{get;private set;}=0;
    [Export] InputController controller;
    public List<Node2D> visibleTargets=new();
    float speed;
    int hp;
    [Export] bool isPlayer=false;
    [Export] int maxHp=2;
    [Export] float maxSpeed = 50f;
    [Export] float maxReverseSpeed = -30f;
    [Export] float acceleration = 20f;
    [Export] float boostSpeedGain = 60f;
    [Export] Cannon cannon;

    Track track{get{return GetParent<Track>();}}

    public override void _Ready()
    {
        base._Ready();
        if (controller){
            controller.train = this;
        } 
        hp = maxHp;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        float dt = (float)delta;
        if (cannon) cannon.Process(dt);
        if (controller && controller.GetFireInput() && cannon.CanFire()){
            FireCannon();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        float dt = (float)delta;
        if (controller){
            float speedIncrease = controller.GetAccelerationInput() * acceleration * dt;
            speedIncrease = Mathf.Clamp(speedIncrease,Mathf.Min(0,maxReverseSpeed-speed),Mathf.Max(0,maxSpeed-speed));
            speed += speedIncrease;
        }
        // slowly reset to clamped speed.
        speed = Mathf.Lerp(speed,Mathf.Clamp(speed,maxReverseSpeed,maxSpeed),0.02f);
        Move(speed*dt);
    }

    void Move(float moveAmt){
        Track nextOutboundTrack = track.GetNextOutboundTrack(Progress);
        Progress += moveAmt;
        if (nextOutboundTrack != null && 
        Progress > nextOutboundTrack.entryData.distance && 
        controller.GetTurnInput() == nextOutboundTrack.entryData.direction){
            //GD.Print(nextSwitch.toTrack);
            Reparent(nextOutboundTrack,false);
            Progress -= nextOutboundTrack.entryData.distance;
        }
        else if (ProgressRatio >= 1f){
            Track dest = track.GetDestinationTrack();
            float dist = track.exitData.distance;
            if (track.exitData.isFinishLine) FinishLap();
            Reparent(dest,false);
            Progress = dist;
        }
    }

    void FinishLap(){
        lapsComplete++;
        if (isPlayer){
            if (lapsComplete >= 3) GameOver();
            else GameManager.instance.UpdateLapCounter(lapsComplete+1);
        }
    }

    void GameOver(){
        GD.Print("GAME OVER!");
        // TODO
    }

    public void Boost(float mult=1f){
        AddSpeed(boostSpeedGain*mult);
    }

    public void AddSpeed(float amt){
        speed += amt;
    }

    public void MultiplySpeed(float mult){
        speed *= mult;
    }

    public void TakeDamage(int dmg=1){
        hp -= dmg;
        if (hp <= 0) QueueFree();
    }

    public void FireCannon(){
        Projectile proj = cannon.OnFire();
        GetTree().Root.GetChild(0).GetNode("ProjectileHolder").AddChild(proj);
        proj.owner = this;
        proj.GlobalPosition = GlobalPosition;
        proj.LinearVelocity = (controller.GetAimTarget() - GlobalPosition).Normalized() * cannon.shotSpeed;
    }

    public void OnRamCollide(Node2D otherBody){
        Train otherTrain = otherBody.GetParentOrNull<Train>();
        if (otherTrain == null) return;
        float speedDiff = speed - otherTrain.speed;
        otherTrain.OnRammed(this,speedDiff + 5f);
        AddSpeed(-speedDiff - 5f);
    }

    public void OnRammed(Train attacker, float hitForce){
        if (attacker == this) return;
        AddSpeed(hitForce);
        TakeDamage();
    }

    public void OnAttacked(Node2D attacker, float hitSpdMult, int damage = 0){
        MultiplySpeed(hitSpdMult);
        if (damage > 0) TakeDamage(damage);
    }

    public void AddVisibleTarget(Node2D target){
        if (target.GetParent() != this) visibleTargets.Add(target);
    }

    public void RemoveVisibleTarget(Node2D target){
        visibleTargets.Remove(target);
    }
}
