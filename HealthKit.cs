using Godot;
using System;

public class HealthKit : Area2D
{
    public int HealthPoints = 5;


    public override void _Ready()
    {
        this.Connect("body_entered", this, "OnBodyEntered");
    }

    // Called when another body enters health kit.
    private void OnBodyEntered(PhysicsBody2D body)
    {
        if (body is PlayerKinematicBody2D player)
        {
            player.Health += this.HealthPoints;
            if (player.Health > 10)
                player.Health = 10;

            this.GetNode<AudioStreamPlayer2D>("/root/EnvironNode2D/HealthRegenSound").Play();
            this.QueueFree();
        }
    }
}
