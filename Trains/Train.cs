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
    float invincibilityTimer=0f;
    public int racePosition; // received from GameManager
    [Export] public bool isPlayer{get;private set;}=false;
    [Export] int maxHp=2;
    [Export] float maxSpeed = 50f;
    [Export] float maxReverseSpeed = -30f;
    [Export] float acceleration = 20f;
    [Export] float boostSpeedGain = 60f;
    [Export] Cannon cannon;
    [Export] HPBar hpBar;
    [Export] public TextureProgressBar reloadProgress;

    Track track{get{return GetParent<Track>();}}

    public override void _Ready()
    {
        base._Ready();
        if (controller){
            controller.train = this;
        } 
        hp = maxHp;
        hpBar.Init(maxHp);

        // not great code but Godot can't export lists right now so this is easiest
        (GetTree().Root.GetChild(0) as GameManager).trains.Add(this);
        if (isPlayer) (GetTree().Root.GetChild(0) as GameManager).player = this;
    }

    public override void _Process(double delta)
    {
        if (!GameManager.instance.raceStarted) return;

        base._Process(delta);
        float dt = (float)delta;
        if (cannon) cannon.Process(dt);
        if (controller && controller.GetFireInput() && cannon.CanFire()){
            FireCannon();
        }
        invincibilityTimer -= dt;
        if (cannon && reloadProgress != null) UpdateReloadProgress();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!GameManager.instance.raceStarted) return;

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
        moveAmt = Math.Max(0f,moveAmt); // game jam - no reverse, since track switching doesn't work in reverse.
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
            bool isFinish = track.exitData.isFinishLine;
            Reparent(dest,false);
            Progress = dist;
            if (isFinish) FinishLap();
        }
    }

    void UpdateReloadProgress(){
        if (reloadProgress != null){
            reloadProgress.Visible = true;
            reloadProgress.Value = (1-((double) cannon.cRechargeTime / cannon.rechargeTime)) * 100;
            reloadProgress.GetNode<Control>("Cannonball").Visible = cannon.cShots >= 1;
        }
    }

    void FinishLap(){
        lapsComplete++;
        if (isPlayer) {
            GameManager.instance.UpdateLapCounter(lapsComplete+1);
        }
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
        if (invincibilityTimer <= 0f){
            hp -= dmg;
            hpBar.UpdateHP(hp);
            invincibilityTimer = .5f;
        } 
        if (hp <= 0){
            if (isPlayer) GameManager.instance.GameOver();
            GameManager.instance.trains.Remove(this);
            QueueFree();
        } 
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
        if (track != otherTrain.track) speedDiff += maxSpeed;
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

    public float GetDistanceAlongTrack(){
        return lapsComplete * 10000f + Progress + track.GetDistanceFromStart();
    }
}
