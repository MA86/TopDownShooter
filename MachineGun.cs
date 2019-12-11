using Godot;
using System;

public class MachineGun : Gun
{
    // Fields
    [Export] public int ClipSize = 30;
    [Export] public float ReloadTime = 2;
    [Export] public float RateOfFire = 1000;   // Per minute
    [Export] public float Theta = 8;
    [Export] public int CurrentRounds = 30;
    [Export] public int Range = 500;

    Timer bulletTimer;
    Timer reloadTimer;
    PackedScene bulletResource;
    private float lastBulletTime = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        bulletResource = GD.Load<PackedScene>("res://BulletRigidBody2D.tscn");

        // Initialize bullet timer.
        bulletTimer = new Timer();
        this.AddChild(bulletTimer);
        float rateOfFirePerSecond = RateOfFire / 60.0f;
        bulletTimer.SetWaitTime(1.0f / rateOfFirePerSecond);
        bulletTimer.Connect("timeout", this, "OnTimeToShoot");

        // Initialize reload timer.
        reloadTimer = new Timer();
        this.AddChild(reloadTimer);
        reloadTimer.Connect("timeout", this, "OnTimeToReload");
        reloadTimer.SetWaitTime(ReloadTime);
        reloadTimer.OneShot = true;

        // default texture
        ImageTexture defaultTexture = new ImageTexture();
        defaultTexture.Load("res://topdown-shooter/PNG/weapon_gun.png");
        this.Texture = defaultTexture;
    }


    // Called when it's time to spawn a bullet.
    void OnTimeToShoot()
    {
        if (this.CurrentRounds <= 0)
            return;

        this.FireBullet();
        this.CurrentRounds -= 1;
    }


    // Spawns a bullet.
    private void FireBullet()
    {
        // If it's not yet time to fire, then don't.
        float timeNeeded = 1.0f / RateOfFire * 60.0f * 1000.0f;
        if (OS.GetTicksMsec() - this.lastBulletTime < timeNeeded)
            return;

        // Create a bullet and position it on the gun.
        BulletRigidBody2D bullet = (BulletRigidBody2D)bulletResource.Instance();
        this.GetParent().GetParent().AddChild(bullet);
        bullet.Rotation = this.GlobalRotation;
        bullet.Rotation += (float)GD.RandRange(Mathf.Deg2Rad(-Theta), Mathf.Deg2Rad(Theta));
        bullet.Position = this.GlobalPosition;
        bullet.MaxDistance = this.Range;

        // Set off the bullet
        bullet.Go();

        this.lastBulletTime = OS.GetTicksMsec();
    }


    // Called when it's time to reload.
    private void OnTimeToReload()
    {
        this.CurrentRounds = this.ClipSize;
    }

    // Pull trigger fires bullet if magazine is full.
    public override void PullTrigger()
    {
        if (CurrentRounds > 0)
            this.FireBullet();
        this.bulletTimer.Start();
    }

    public override void ReleaseTrigger()
    {
        this.bulletTimer.Stop();
    }


    // Reloads the gun. Does nothing if the gun is already reloading.
    public void Reload()
    {
        if (reloadTimer.IsStopped()){
            reloadTimer.Start();
        }
    }
}
