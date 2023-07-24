using Godot;
using System;

public partial class PauseManager : Node2D
{
    [Export] Control pauseMenu;
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed("Pause")) TogglePause();
    }

    public void TogglePause(){
        if (GetTree().Paused) Unpause();
        else Pause();
    }

    public void Pause(){
        if (GameManager.instance.raceFinished || GameManager.instance.onStartMenu) return;
        GetTree().Paused = true;
        pauseMenu.Visible = true;
    }

    public void Unpause(){
        GetTree().Paused = false;
        pauseMenu.Visible = false;
    }
}
