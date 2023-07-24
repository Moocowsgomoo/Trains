using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node2D
{
    public static GameManager instance;
    public bool raceStarted = false;
    public bool raceFinished = false;
    public bool onStartMenu = true;
    public List<Train> trains=new();
    public Train player;
    [Export] Label racePositionText;
    [Export] Label lapCounter;
    [Export] Label startCountdown;
    [Export] Control startMenu;
    [Export] Control gameOverScreen;
    [Export] Timer targetSpawnTimer;
    [Export] PauseManager pauseManager;
    [Export] public int laps{get;private set;} = 3;

    public override void _Ready()
    {
        base._Ready();
        if (instance != null && instance != this && IsInstanceValid(instance)) QueueFree();
        instance = this;
        startMenu.Visible = true;
        UpdateLapCounter(1);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (trains.Count == 1 && trains[0].isPlayer) GameWin();
    }

    public void StartCountdown(){
        startMenu.Visible = false;
        onStartMenu = false;
        startCountdown.GetNode<AnimationPlayer>("AnimationPlayer").Play("PlayStartCountdown");
        targetSpawnTimer.Start();
    }

    public void StartRace(){
        raceStarted = true;
    }

    // called from timer
    public int UpdatePositions(){
        trains.Sort((x,y)=>Mathf.Sign(y.GetDistanceAlongTrack() - x.GetDistanceAlongTrack()));
        for (int i=0;i<trains.Count;i++){
            trains[i].racePosition = i+1;
        }
        if (player != null && IsInstanceValid(player)){
            racePositionText.Text = GetPositionText(player.racePosition);
            return player.racePosition;
        }
        return 0;
    }

    string GetPositionText(int pos){
        if (pos == 1) return "1st";
        else if (pos == 2) return "2nd";
        else if (pos == 3) return "3rd";
        else return $"{pos}th";
    }

    public void UpdateLapCounter(int lap){
        if (lap > laps) GameWin();
        else lapCounter.Text = $"Lap {lap}/{laps}";
    }

    public void GameOver(){
        if (raceFinished) return;
        raceFinished = true;
        gameOverScreen.GetNode<AnimationPlayer>("AnimationPlayer").Play("GameOverScreenAppear");
    }

    public void GameWin(){
        if (raceFinished) return;
        raceFinished = true;
        int pos = UpdatePositions();
        gameOverScreen.GetNode<Control>("Content").GetNode<Label>("Title").Text = $"{GetPositionText(pos)} place";
        gameOverScreen.GetNode<Control>("Content").GetNode<Label>("Description").Text = pos == 1 ? "Congratulations!" : "Can you get 1st?";
        gameOverScreen.GetNode<AnimationPlayer>("AnimationPlayer").Play("GameOverScreenAppear");
    }

    public void Restart(){
        pauseManager.Unpause();
        GetTree().ReloadCurrentScene();
    }
}
