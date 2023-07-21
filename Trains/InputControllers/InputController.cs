using Godot;
using System;

public abstract partial class InputController : Resource
{
    public Train train;
    public abstract float GetAccelerationInput();
    public abstract SwitchDirection GetTurnInput();
    public abstract Vector2 GetAimTarget();
    public abstract bool GetFireInput();
    
    public static implicit operator bool(InputController c){
        return c != null;
    }
}
