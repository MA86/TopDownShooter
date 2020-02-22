using Godot;
using System;

public class CellGui : Panel
{
    [Signal] public delegate void CellClicked(CellGui cell);

    public Gun gun;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.GetNode<TextureButton>("TextureButton").Connect("pressed", this, "OnButtonPressed");
    }

    // Executed when the TextureButton is pressed.
    void OnButtonPressed()
    {
        this.EmitSignal("CellClicked", this);
    }

    public void SetGun(Gun gun)
    {
        this.gun = gun;
        this.GetNode<TextureButton>("TextureButton").TextureNormal = gun.Icon;
            
    }
}
