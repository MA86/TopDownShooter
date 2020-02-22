using Godot;
using System;

public class BulletRigidBody2D : RigidBody2D
{
    [Export] public float Speed = 800;
    [Export] public float Life = 1;             // how long (in seconds) the bullet lives (before it is deleted)
    [Export] public float Damage = 2;
    [Export] public float MaxDistance = 500;

    Vector2 startPosition;
    Timer bulletTimer;


    // Called when the node enters the node tree.
    public override void _Ready()
    {
        // Bullet disappears after x seconds.
        bulletTimer = new Timer();
        this.AddChild(bulletTimer);
        bulletTimer.Connect("timeout", this, "OnBulletTimeout");
        bulletTimer.SetWaitTime(this.Life);
        bulletTimer.Start();

        // Signal when entering a body.
        this.Connect("body_entered", this, "OnBulletBodyEntered");
        this.ContactMonitor = true;
        this.ContactsReported = 10;

        // Make bullet invisible initially.
        this.Hide();
    }


    // Called every frame.
    public override void _PhysicsProcess(float delta)
    {
        // Bullet range.
        float distanceTravelled = this.startPosition.DistanceTo(this.Position);

        if (distanceTravelled > this.MaxDistance)
        {
            this.QueueFree();
        }
    }


    // Timeout function for destroying bullet.
    private void OnBulletTimeout()
    {
        this.QueueFree();
    }


    // Fires the bullet.
    public void Go()
    {
        this.startPosition = this.Position;
        this.Show();
        this.ApplyImpulse(this.Position, this.GlobalTransform.x.Normalized() * Speed);
    }


    // Called to cause damage to target.
    private void OnBulletBodyEntered(Node body)
    {
        if (body is ZombieKinematicBody2D zombie)
        {
            zombie.ApplyDamage(this.Damage);
            this.QueueFree();
        }
    }
}
