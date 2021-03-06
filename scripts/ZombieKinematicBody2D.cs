using Godot;
using System;

public class ZombieKinematicBody2D : KinematicBody2D
{
	[Export] public float MoveSpeed = 50;
	[Export] public float Health = 10;
	[Export] public float DamageCaused = 2;
	[Export] public bool IsActive = true;
	public Node2D TargetNode;


	// Called when the zombie first enters the scene tree.
	public override void _Ready()
	{
		// Add the zombie to the collection, and have it target the player
		EnvironNode2D env = this.GetNode<EnvironNode2D>("/root/EnvironNode2D");
		env.Zombies.Add(this);

		// Play the idle zombie animation
		AnimatedSprite anim = this.GetNode<AnimatedSprite>("ZombieAnimatedSprite");
		anim.Animation = "walk";
		anim.Play();


	}


	// Called every frame.
	public override void _Process(float delta)
	{
		// If zombie is ACTIVE and has a TARGET, then call this function, otherwise the game will crash.
		if (IsActive == true && TargetNode != null)
		{
			this.MakeNextMove(delta);
		}
	}


	// The zombie will either move closer to the target or attack the target.
	private void MakeNextMove(float delta)
	{
		// Get animation reference
		AnimatedSprite animation = this.GetNode<AnimatedSprite>("ZombieAnimatedSprite");

		// Look at the target
		this.LookAt(TargetNode.GlobalPosition);

		// Get displacement vector from zombie to target, and then move
		Vector2 dispVector = TargetNode.GlobalPosition - this.GlobalPosition;
		dispVector = dispVector.Normalized();
		KinematicCollision2D collisionData = this.MoveAndCollide(dispVector * delta * MoveSpeed);

		// If there is a collision with the target, attack the target
		if (collisionData != null)
		{
			if (collisionData.Collider is PlayerKinematicBody2D p)
			{
				animation.Animation = "hurt";
				animation.Play();

				if (animation.Frame == 0)
					p.ApplyDamage(this.DamageCaused);

				if (animation.Frame == animation.Frames.GetFrameCount("hurt") - 1)
					animation.Frame = 0;
			}
		}
		else
		{
			// Play the walking animation 
			if (animation.Animation == "hurt")
			{
				if (animation.Frame == animation.Frames.GetFrameCount("hurt") - 1)
				{
					animation.Animation = "walk";
					animation.Play();
				}
			}

		}
	}


	// Apply the damage caused to this zombie.
	public void ApplyDamage(float amount)
	{
		// Subract damage from health
		this.Health -= amount;

		// Play animation. TODO: modify MakeNextMove to make this work. too much power in 'else'
		AnimatedSprite anim = this.GetNode<AnimatedSprite>("ZombieAnimatedSprite");
		AudioStreamPlayer2D audio = this.GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		audio.Play();
		anim.Animation = "hurt";
		anim.Frame = 0;
		anim.Play();

		// If health is zero, zombie dies
		if (this.Health <= 0)
		{
			//anim.Animation = "death";
			//anim.Play();
			//anim.SetFrame(0);

			this.QueueFree();
		}
	}
}
