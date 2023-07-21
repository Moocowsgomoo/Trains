using Godot;
using System;

[GlobalClass]
public partial class PlayerInputController : InputController
{
    public override float GetAccelerationInput(){
        return Input.GetAxis("Reverse","Forward");
    }
    public override SwitchDirection GetTurnInput(){
        if (Input.GetActionStrength("Left") > 0f) return SwitchDirection.LEFT;
        if (Input.GetActionStrength("Right") > 0f) return SwitchDirection.RIGHT;
        return SwitchDirection.NONE;
    }
    public override Vector2 GetAimTarget(){
        return train.GetGlobalMousePosition();
    }
    public override bool GetFireInput(){
        return Input.IsActionPressed("Fire");
    }
}
