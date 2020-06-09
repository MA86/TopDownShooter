using Godot;
using System;

public class Barrel : Mine
{
	private PlayerKinematicBody2D player;

	public override void _Ready()
	{
		base._Ready();
		this.Connect("body_exited", this, "OnBodyExit");
	}

	public override void _Process(float delta)
	{
		this.player = this.GetNode<EnvironNode2D>("/root/EnvironNode2D").Player; // TODO figure out a way not to do this - very inefficient (can't put it in ready due to order of ready call)
	}

	// Called when someone shoots the barrel.
	public override void OnBodyEntered(PhysicsBody2D body)
	{
		if (body is BulletRigidBody2D bullet)
		{
			FXs fx = this.GetNode<FXs>("/root/EnvironNode2D/FX");

			fx.PlayMineExplosionAnimAndSound(this.Position);
			this.SpawnBullets();
			this.QueueFree();
		}

		if (body is PlayerKinematicBody2D player)
		{
			player.OverBarrel = this;
		}
	}

	private void OnBodyExit(PhysicsBody2D body)
	{
		if (body is PlayerKinematicBody2D player)
		{
			player.OverBarrel = null;
		}
	}
}
