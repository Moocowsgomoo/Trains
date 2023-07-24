using Godot;
using System;

[GlobalClass]
public partial class Cannon : Resource
{
    [Export] PackedScene projectile;
    [Export] int maxStoredShots=1;
    [Export] public float rechargeTime;
    [Export] float shotCooldownTime;
    [Export] public float shotSpeed = 200f;
    [Export] float trainSpdMult = 0.7f;

    public int cShots;
    public float cRechargeTime;
    float cShotCooldownTime;

    public void Process(float dt){
        cRechargeTime -= dt;
        cShotCooldownTime -= dt;
        if (cRechargeTime <= 0f && cShots < maxStoredShots){
            AddStoredShot();
        }
    }

    void AddStoredShot(int amt = 1, bool resetRecharge = true){
        cShots += amt;
        if (resetRecharge) cRechargeTime = rechargeTime;
    }

    public bool CanFire(){
        return cShots > 0 && cShotCooldownTime < 0f;
    }

    public Projectile OnFire(){
        cShots--;
        cShotCooldownTime = shotCooldownTime;
        Projectile p = ResourceLoader.Load<PackedScene>(projectile.ResourcePath).Instantiate<Projectile>();
        p.trainSpdMult = trainSpdMult;
        return p;
    }

    public static implicit operator bool(Cannon c){
        return c != null;
    }
}
