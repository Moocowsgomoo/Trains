using Godot;
using System;

public partial class Projectile : Area2D
{
    public Train owner;
    public float trainSpdMult;
    public Vector2 LinearVelocity;

    public void OnHitSomething(Node2D target){
        Train hitTrain = target.GetParentOrNull<Train>();
        if (hitTrain != null){
            if (hitTrain != owner){
                hitTrain.OnAttacked(this,trainSpdMult);
                DestroyThis();
            }
        }
        else{
            owner.Boost();
            target.QueueFree();
            DestroyThis();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        float dt = (float)delta;
        Position += LinearVelocity*dt;
    }

    public void DestroyThis(){
        QueueFree();
    }
}
