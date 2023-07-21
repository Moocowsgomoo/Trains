using Godot;
using System;

public partial class TargetHolder : Control
{
    [Export] PackedScene boostTarget;

    RandomNumberGenerator rng = new RandomNumberGenerator();

    public void OnTimerTimeout(){
        Node2D newTarget = ResourceLoader.Load<PackedScene>(boostTarget.ResourcePath).Instantiate<Node2D>();
        AddChild(newTarget);
        newTarget.Position = new Vector2(rng.RandfRange(0,Size.X),rng.RandfRange(0,Size.Y));
    }
}
