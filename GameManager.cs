using Godot;
using System;

public partial class GameManager : Node2D
{
    public static GameManager instance;
    [Export] Label lapCounter;

    public override void _Ready()
    {
        base._Ready();
        if (instance != null && instance != this) QueueFree();
        instance = this;
    }

    public void UpdateLapCounter(int lap){
        lapCounter.Text = $"Lap {lap}";
    }
}
