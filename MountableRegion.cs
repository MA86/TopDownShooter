using Godot;
using System;

public class MountableRegion : Area2D
{

    [Export] public float Arc = 75;     // how far turning this mountable region allows

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Connect("body_entered", this, "OnBodyEntered");
        this.Connect("body_exited", this, "OnBodyExited");

    }

    void OnBodyEntered(PhysicsBody2D body)
    {
        if (body is PlayerKinematicBody2D player)
        {
            player.MountableRegion = this;
        }
    }

    void OnBodyExited(PhysicsBody2D body)
    {
        if (body is PlayerKinematicBody2D player)
        {
            player.MountableRegion = null;
        }
    }

}
