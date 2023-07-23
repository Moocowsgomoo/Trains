using Godot;
using System;

[GlobalClass]
public partial class BasicAIInputController : InputController
{
    RandomNumberGenerator rng = new RandomNumberGenerator();
    public override float GetAccelerationInput(){
        return 1f;
    }
    public override SwitchDirection GetTurnInput(){
        float n = rng.Randf();
        if (n <= 0.33f) return SwitchDirection.LEFT;
        else if (n <= 0.67f) return SwitchDirection.NONE;
        else return SwitchDirection.RIGHT;
    }
    public override Vector2 GetAimTarget(){
        if (train.visibleTargets.Count == 0) return Vector2.Zero;
        return train.visibleTargets[rng.RandiRange(0,train.visibleTargets.Count-1)].GlobalPosition;
    }
    public override bool GetFireInput(){
        return rng.Randf() > 0.9995f;
    }
}
