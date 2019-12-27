using Godot;
using System;

public class Barrel : Mine
{
    private bool collidingWithPlayer = false;
    private PlayerKinematicBody2D player;

    public override void _Ready()
    {
        base._Ready();
        this.Connect("body_exited", this, "OnBodyExit");
    }

    public override void _Process(float delta)
    {
        var player = this.GetNode<EnvironNode2D>("/root/EnvironNode2D").Player;
        if (Input.IsActionJustPressed("pickup") && this.collidingWithPlayer && player.Barrel == null)
        {
            // Pick up barrel
            this.Position = new Vector2(0, 0);
            this.GetParent().RemoveChild(this);
            player.AddChild(this);
            player.Barrel = this;
        }
    }

    // Called when someone shoots the barrel.
    public override void OnBodyEntered(PhysicsBody2D body)
    {
        if (body is BulletRigidBody2D bullet)
        {
            this.SpawnBullet();
            this.QueueFree();
        }

        if (body is PlayerKinematicBody2D player)
        {
            this.collidingWithPlayer = true;

        }
    }

    private void OnBodyExit(PhysicsBody2D body)
    {
        if (body is PlayerKinematicBody2D player)
        {
            this.collidingWithPlayer = false;
        }
    }
}
