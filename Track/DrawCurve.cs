using Godot;
using System;

public partial class DrawCurve : Line2D
{
    public override void _Ready()
    {
        Points = GetParent<Path2D>().Curve.Tessellate();
        base._Draw();
    }
}
