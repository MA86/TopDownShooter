using Godot;
using System;

public class CellGui : TextureButton
{
    [Signal] public delegate void CellClicked(CellGui cell);

    public Gun gun;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Connect("pressed", this, "OnButtonPressed");
    }

    // Executed when the TextureButton is pressed.
    void OnButtonPressed()
    {
        this.EmitSignal("CellClicked", this);
    }

    public void SetGun(Gun gun)
    {
        this.gun = gun;
        this.TextureNormal = gun.Texture;
    }
}
