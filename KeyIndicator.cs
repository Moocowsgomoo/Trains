using Godot;
using System;

public partial class KeyIndicator : Sprite2D
{
    [Export] string display;
    [Export] string controlName;

    public override void _Ready()
    {
        base._Ready();
        GetNode<Label>("Label").Text = display;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed(controlName)) GetNode<AnimationPlayer>("AnimationPlayer").Play("Highlight");
        if (Input.IsActionJustReleased(controlName)) GetNode<AnimationPlayer>("AnimationPlayer").Play("Unhighlight");
    }
}
