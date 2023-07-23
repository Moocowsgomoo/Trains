using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node2D
{
    public static GameManager instance;
    public bool raceStarted = false;
    public bool raceFinished = false;
    public List<Train> trains=new();
    [Export] Label racePositionText;
    [Export] Label lapCounter;
    [Export] Label startCountdown;
    [Export] Control gameOverScreen;
    [Export] public int laps{get;private set;} = 3;

    public override void _Ready()
    {
        base._Ready();
        if (instance != null && instance != this) QueueFree();
        instance = this;

        startCountdown.GetNode<AnimationPlayer>("AnimationPlayer").Play("PlayStartCountdown");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
    }

    public void StartRace(){
        raceStarted = true;
    }

    // called from timer
    public void UpdatePositions(){

        trains.Sort((x,y)=>Mathf.Sign(y.GetDistanceAlongTrack() - x.GetDistanceAlongTrack()));
        for (int i=0;i<trains.Count;i++){
            if (trains[i].isPlayer) UpdatePositionText(i+1);
        }

        void UpdatePositionText(int pos){
        if (pos == 1) racePositionText.Text = "1st";
        else if (pos == 2) racePositionText.Text = "2nd";
        else if (pos == 3) racePositionText.Text = "3rd";
        else racePositionText.Text = $"{pos}th";
    }
    }

    public void UpdateLapCounter(int lap){
        lap = Mathf.Min(lap,laps);
        lapCounter.Text = $"Lap {lap}/{laps}";
        // TODO Game Over if done all laps
    }

    public void GameOver(){
        raceFinished = true;
        gameOverScreen.GetNode<AnimationPlayer>("AnimationPlayer").Play("GameOverScreenAppear");
    }
}
