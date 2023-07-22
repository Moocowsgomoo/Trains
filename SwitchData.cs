using Godot;
using System;

[GlobalClass]
public partial class SwitchData : Resource
{
    [Export] public NodePath track;
    [Export] public float distance;
    [Export] public SwitchDirection direction;
    [Export] public bool isFinishLine=false;
}
