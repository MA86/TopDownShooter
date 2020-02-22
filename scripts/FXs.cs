using Godot;
using System;

/// <summary>
/// FX scene (and hence its script) is a scene which deals with all 
/// animations and sound effects for all other scenes. This design 
/// decision allow us to free objects while their animations are 
/// being played.
/// </summary>
public class FXs : Node2D
{
    AnimatedSprite animations;      // Used to reference animations in FX scene.
    AudioStreamPlayer2D sound;      // Used to reference audio in FX scene.

    // When FX enters the scene tree.
    public override void _Ready()
    {
        // Get a reference to AnimatedSprite inside FX scene.
        animations = this.GetNode<AnimatedSprite>("Animations");

        // Get a reference to AudioStreamPlayer2D inside FX scene.
        sound = this.GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
    }

    // Mine explosion and sound.
    public void PlayMineExplosionAnimAndSound(Vector2 position)
    {
        animations.Position = position;
        sound.Play();
        animations.Play("mine_explosion");
        animations.SetFrame(0);
    }
}
