using Godot;
using System;

public partial class HPBar : HBoxContainer
{
    [Export] PackedScene heartPrefab;
    [Export] AnimationPlayer animator;

    public void Init(int hp){
        for(int i=0;i<hp;i++){
            AddChild(ResourceLoader.Load<PackedScene>(heartPrefab.ResourcePath).Instantiate<Control>());
        }
    }

    public void UpdateHP(int hp){
        for(int i=0;i<GetChildCount();i++){
            GetChild(i).GetNode<Control>("Heart").Visible = i < hp;
        }
        animator.Play("FadeOut");
    }
}
