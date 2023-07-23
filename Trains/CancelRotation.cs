using Godot;
using System;

public partial class CancelRotation : Node2D
{
    [Export] Node2D target;

    public override void _Process(double delta)
    {
        base._Process(delta);
        target.GlobalRotation = 0f;
    }
}
