using Godot;
using System;

[GlobalClass]
public partial class BasicAIInputController : InputController
{
    Random rng = new Random();
    public override float GetAccelerationInput(){
        return 1f;
    }
    public override SwitchDirection GetTurnInput(){
        return SwitchDirection.NONE;
    }
    public override Vector2 GetAimTarget(){
        return Vector2.Zero;
    }
    public override bool GetFireInput(){
        return false;//rng.Next() > 0.999f;
    }
}
