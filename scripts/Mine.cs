using Godot;
using System;

public class Mine : Area2D
{
	PackedScene bulletScene;

	[Export] public int NumberOfBulletsGenerated = 10;
	[Export] public float BlastRadius = 300;


	public override void _Ready()
	{
		bulletScene = GD.Load<PackedScene>("res://scenes/BulletRigidBody2D.tscn");
		this.Connect("body_entered", this, "OnBodyEntered");
	}

	// Spawn abunch of bullets originating from the mine.
	protected void SpawnBullets()
	{
		for (double i = 0; i < 360; i += (360.0 / this.NumberOfBulletsGenerated))
		{
			BulletRigidBody2D bullet = (BulletRigidBody2D)bulletScene.Instance();
			this.GetNode<Node2D>("/root/EnvironNode2D/OnGround").AddChild(bullet);
			bullet.GlobalPosition = this.GlobalPosition;
			bullet.GlobalRotation = (float)i;
			bullet.Speed = (float)GD.RandRange(1000, 1400);
			bullet.MaxDistance = this.BlastRadius;
			bullet.Go();
		}
	}

	// Called when someone steps over the mine.
	public virtual void OnBodyEntered(PhysicsBody2D body)
	{
		if ((body is ZombieKinematicBody2D zombie) || (body is PlayerKinematicBody2D p))
		{
			// explode

			// note: TODO minor bug: if you set NumberOfBulletsGenerated to 4, you will see that the
			// mine does not distribute the bullets equal around

			// Reference FX script and play sound/animation.
			FXs fx = this.GetNode<FXs>("/root/EnvironNode2D/FX");

			fx.PlayMineExplosionAnimAndSound(this.Position);
			this.SpawnBullets();  // Explode, then free.
			this.QueueFree();
		}
	}
}
