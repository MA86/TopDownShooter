using Godot;
using System;

public class HealthKit : Area2D
{
    [Export] public int HealthPoints = 5;


    public override void _Ready()
    {
        this.Connect("body_entered", this, "OnBodyEntered");
    }

    // Called when another body enters health kit.
    private void OnBodyEntered(PhysicsBody2D body)
    {
        if (body is PlayerKinematicBody2D player)
        {
            player.AddToHealth(this.HealthPoints);
            this.GetNode<AudioStreamPlayer2D>("/root/EnvironNode2D/HealthRegenSound").Play();
            this.QueueFree();
        }
    }
}
