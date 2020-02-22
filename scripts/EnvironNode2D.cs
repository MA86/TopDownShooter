using Godot;
using System;
using System.Collections.Generic;

public class EnvironNode2D : Node2D
{
    // Fields
    public List<ZombieKinematicBody2D> Zombies = new List<ZombieKinematicBody2D>();
    public PlayerKinematicBody2D Player;
    public Inventory PlayerBag;


    // Called when the EnvironNode2D enters the scene tree for the first time.
    public override void _Ready()
    {
        Player = this.GetNode<PlayerKinematicBody2D>("OnGround/PlayerKinematicBody2D");
        PlayerBag = this.GetNode<Inventory>("CanvasLayer/Inventory");

        foreach (var zombie in this.Zombies)
            zombie.TargetNode = this.Player;

        // Listen to inventory click event.
        PlayerBag.Connect("CellClicked", this, "OnCellClicked");

        // Setup crosshair on the mouse icon.
        Input.SetDefaultCursorShape(Input.CursorShape.Cross);
    }

    private void OnCellClicked(CellGui cellClicked)
    {
        Player.EquipGun(cellClicked.gun);
    }
    // Called every frame
    public override void _Process(float delta)
    {
        // position the camera wherever the player is at.
        //camera.GlobalPosition = this.Player.GlobalPosition;
    }
}
