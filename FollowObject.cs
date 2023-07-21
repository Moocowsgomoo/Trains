using Godot;
using System;

public partial class FollowObject : Node2D
{
    [Export] Node2D target;
    [Export] float forwardOffset;

    public override void _Process(double delta)
    {
        base._Process(delta);
        GlobalPosition = target.GlobalPosition + Vector2.FromAngle(target.GlobalRotation) * forwardOffset;
        GlobalRotation = target.GlobalRotation + Mathf.DegToRad(90f);
    }
}
