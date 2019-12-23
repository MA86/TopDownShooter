using Godot;
using System;

public class Barrel : Mine
{

    // Called when someone shoots the barrel.
    public override void OnBodyEntered(PhysicsBody2D body)
    {
        if (body is BulletRigidBody2D bullet)
        {
            this.SpawnBullet();
            this.QueueFree();
            
        }
    }
}
