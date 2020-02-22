using Godot;
using System;

public class GunCrate : RigidBody2D
{
    // Fields
    [Export] PackedScene GunScene;
    public Gun Gun;


    // Called when the GunCrate enters the scene tree for the first time.
    public override void _Ready()
    {
        this.ContactMonitor = true;
        this.ContactsReported = 10;
        this.Gun = GunScene.Instance() as Gun;
    }
}
