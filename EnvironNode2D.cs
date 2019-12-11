using Godot;
using System;
using System.Collections.Generic;

public class EnvironNode2D : Node2D
{
    // Fields
    public List<ZombieKinematicBody2D> Zombies = new List<ZombieKinematicBody2D>();
    public PlayerKinematicBody2D Player;  // This reference is set by the players blueprint
    public Inventory PlayerBag;

    // Called when the EnvironNode2D enters the scene tree for the first time.
    public override void _Ready()
    {
        Player = this.GetNode<PlayerKinematicBody2D>("PlayerKinematicBody2D");
        PlayerBag = this.GetNode<Inventory>("CanvasLayer/Inventory");

        foreach (var zombie in this.Zombies)
            zombie.TargetNode = this.Player;
    }


    // Called every frame
    public override void _Process(float delta)
    {
        // position the camera wherever the player is at.
        //camera.GlobalPosition = this.Player.GlobalPosition;
    }
}
