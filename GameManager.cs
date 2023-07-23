using Godot;
using System;

public partial class GameManager : Node2D
{
    public static GameManager instance;
    public bool raceStarted = false;
    [Export] Label lapCounter;
    [Export] Label startCountdown;

    public override void _Ready()
    {
        base._Ready();
        if (instance != null && instance != this) QueueFree();
        instance = this;

        startCountdown.GetNode<AnimationPlayer>("AnimationPlayer").Play("PlayStartCountdown");
    }

    public void StartRace(){
        raceStarted = true;
    }

    public void UpdateLapCounter(int lap){
        lapCounter.Text = $"Lap {lap}";
    }
}
