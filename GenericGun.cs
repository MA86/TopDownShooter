using Godot;
using System;

public class GenericGun : Gun
{
    // Fields
    [Export] public int ClipSize = 30;
    [Export] public float ReloadTime = 2;
    [Export] public float RateOfFire = 1000;   // Per minute
    [Export] public float Theta = 4;            // Arch of fire when gun is at it's most accurate
    [Export] public float MaxTheta = 10;        // Arch of fire when gun is at it's most inaccurate
    [Export] public int CurrentRounds = 30; 
    [Export] public int Range = 500;            // how far bullets travel
    [Export] bool Auto = true;                  // Auto (true) or SemiAuto (false)
    [Export] float BulletSpeed = 800;
    [Export] int NumBulletsPerShot = 1;         // Normally you'd want this to be 1, set it higher to create a "shotgun" like effect
    [Export] float StaggerDistance = 0;         // When bullets are spanwed, how much random forward movement they get (again, shotgun like effect)  
    [Export] PackedScene BulletUsed;

    private Timer bulletTimer;
    private Timer reloadTimer;
    private Timer canShootTimer;
    private Timer accuracyTimer;
    private float lastBulletTime = 0;
    private bool canShoot = true;
    private bool shooting = false;
    private float immediateTheta;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.immediateTheta = this.Theta;

        // by default, use this bullet
        if (BulletUsed == null)
            BulletUsed = GD.Load<PackedScene>("res://BulletRigidBody2D.tscn");

        // Initialize bullet timer.
        bulletTimer = new Timer();
        this.AddChild(bulletTimer);
        float rateOfFirePerSecond = RateOfFire / 60.0f;
        bulletTimer.SetWaitTime(1.0f / rateOfFirePerSecond);
        bulletTimer.Connect("timeout", this, "OnTimeToShoot");

        // Initialize can shoot timer (only used in semi auto mode)
        canShootTimer = new Timer();
        this.AddChild(canShootTimer);
        canShootTimer.SetWaitTime(1.0f / rateOfFirePerSecond);
        canShootTimer.OneShot = true;
        canShootTimer.Connect("timeout", this, "OnCanShoot");

        // Initialize accuracy timer
        accuracyTimer = new Timer();
        this.AddChild(accuracyTimer);
        accuracyTimer.SetWaitTime(0.25f);
        accuracyTimer.Connect("timeout", this, "OnMakeAccurate");
        accuracyTimer.Start();

        // Initialize reload timer.
        reloadTimer = new Timer();
        this.AddChild(reloadTimer);
        reloadTimer.Connect("timeout", this, "OnTimeToReload");
        reloadTimer.SetWaitTime(ReloadTime);
        reloadTimer.OneShot = true;
    }

    void OnMakeAccurate()
    {
        if (this.shooting)
            return;

        if (this.immediateTheta > this.Theta)
            this.immediateTheta = this.immediateTheta / 2.0f;
        if (this.immediateTheta < this.Theta)
            this.immediateTheta = this.Theta;
        this.Rotation = 0;
    }

    void OnCanShoot()
    {
        this.canShoot = true;
    }


    // Called when it's time to spawn a bullet.
    void OnTimeToShoot()
    {
        if (this.CurrentRounds <= 0)
        {
            this.shooting = false;
            return;
        }

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

        for (int i = 0; i < this.NumBulletsPerShot; i++)
        {
            BulletRigidBody2D bullet = (BulletRigidBody2D)this.BulletUsed.Instance();
            this.GetNode<Node2D>("/root/EnvironNode2D/OnGround").AddChild(bullet);
            this.Rotation = 0;
            this.Rotation += (float)GD.RandRange(Mathf.Deg2Rad(-this.immediateTheta), Mathf.Deg2Rad(this.immediateTheta));
            bullet.Rotation = this.GlobalRotation;
            bullet.Position = this.GlobalPosition;
            bullet.Position = this.GetNode<Position2D>("Position2D").GlobalPosition;
            bullet.Position +=  bullet.GlobalTransform.x * (float)GD.RandRange(0, this.StaggerDistance);
            bullet.Speed = this.BulletSpeed;
            bullet.MaxDistance = this.Range;

            // Set off the bullet
            bullet.Go();
        }

        this.GetNode<AudioStreamPlayer2D>("BulletSound").Play();

        this.lastBulletTime = OS.GetTicksMsec();
        this.immediateTheta += 2.0f;
        if (this.immediateTheta > this.MaxTheta)
            this.immediateTheta = this.MaxTheta;
    }


    // Called when it's time to reload.
    private void OnTimeToReload()
    {
        this.CurrentRounds = this.ClipSize;
    }

    // Pull trigger fires bullet if magazine is full.
    public override void PullTrigger()
    {
        // auto
        if (this.Auto)
        {
            if (CurrentRounds > 0)
            {
                this.FireBullet();
                this.shooting = true;
            }
            this.bulletTimer.Start();
        }
        // semi-auto
        else
        {
            if (this.canShoot)
            {
                this.FireBullet();
                this.canShoot = false;
                this.canShootTimer.Start();
            }
        }
    }

    public override void ReleaseTrigger()
    {
        this.bulletTimer.Stop();
        this.shooting = false;
    }


    // Reloads the gun. Does nothing if the gun is already reloading.
    public void Reload()
    {
        if (reloadTimer.IsStopped())
        {
            this.GetNode<AudioStreamPlayer2D>("ReloadSound").Play();
            reloadTimer.Start();
        }
    }
}
