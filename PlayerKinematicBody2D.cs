using Godot;
using System;

public class PlayerKinematicBody2D : KinematicBody2D
{
    // Modify player attributes.
    [Export] public float MoveSpeed = 120;
    [Export] public float Health = 10;
    [Export] public float DamageCaused = 0;

    Vector2 movementVector;
    GenericGun gun;

    // Called when this player enters the scene.
    public override void _Ready()
    {
        // not used
    }

    // Equip weapon.
    public void EquipGun(Gun gun)
    {
        this.GetNode<AnimatedSprite>("AnimatedSprite").Animation = "hand";

        this.RemoveChild(this.gun);
        this.gun = gun as GenericGun;
        this.AddChild(this.gun);
    }

    // Called every frame.
    public override void _PhysicsProcess(float delta)
    {
        // Always look at the position of the mouse.
        Vector2 mousePos = this.GetNode<Camera2D>("Camera2D").GetGlobalMousePosition();
        this.LookAt(mousePos);

        // Based on the arrow key input, move.
        CalculateMovementVector(delta);     // TODO refactor (have CalculateMovementVector return the new vector)
        KinematicCollision2D collisionInfo = this.MoveAndCollide(movementVector, false);
        if (collisionInfo != null)
        {
            // What to do when collided with a gun crate.
            if (collisionInfo.Collider is GunCrate crate)
            {
                // Add the gun in the inventory.
                Inventory inventory = this.GetNode<Inventory>("/root/EnvironNode2D/CanvasLayer/Inventory");
                inventory.AddGun(crate.Gun);
                crate.QueueFree();
            }
        }

        // Reload gun.
        if (Input.IsKeyPressed((int)KeyList.R))
        {
            gun.Reload();
        }
    }

    // ['Priority' event handling... Because mouse has dual use.]
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            // pressed
            if ((mouseEvent.Pressed) && (mouseEvent.ButtonIndex == (int)ButtonList.Left))
            {
                if(this.gun == null)
                    return;
                
                this.gun.PullTrigger();
                GetTree().SetInputAsHandled();
                return;
            }

            // released
            if ((!mouseEvent.Pressed) && (mouseEvent.ButtonIndex == (int)ButtonList.Left))
            {
                if (this.gun == null)
                    return;

                this.gun.ReleaseTrigger();
                GetTree().SetInputAsHandled();
                return;
            }

            // wheel
            if ((mouseEvent.Pressed) && (mouseEvent.ButtonIndex == (int)ButtonList.WheelUp || mouseEvent.ButtonIndex == (int)ButtonList.WheelDown))
            {
                Camera2D camera = this.GetNode<Camera2D>("/root/EnvironNode2D/OnGround/PlayerKinematicBody2D/Camera2D");
                float currentXZoom = camera.Zoom.x;
                float currentYZoom = camera.Zoom.y;
                float AMOUNT = 0.10f;

                float ZOOM_MAX_IN = 0.5f;
                float ZOOM_MAX_OUT = 2;

                // wheel up
                if (mouseEvent.ButtonIndex == (int)ButtonList.WheelUp)
                    camera.Zoom = new Vector2(currentXZoom - currentXZoom * AMOUNT, currentYZoom - currentYZoom * AMOUNT);
                // wheel down
                if (mouseEvent.ButtonIndex == (int)ButtonList.WheelDown)
                    camera.Zoom = new Vector2(currentXZoom + currentXZoom * AMOUNT, currentYZoom + currentYZoom * AMOUNT);

                // cap zoom
                if (camera.Zoom.x < ZOOM_MAX_IN)
                    camera.Zoom = new Vector2(ZOOM_MAX_IN, ZOOM_MAX_IN);
                if (camera.Zoom.x > ZOOM_MAX_OUT)
                    camera.Zoom = new Vector2(ZOOM_MAX_OUT, ZOOM_MAX_OUT);

                GetTree().SetInputAsHandled();
                return;
            }
        }
    }

    // Calculates a movement vector from arrow keys.
    public void CalculateMovementVector(float delta)
    {
        // By how much to move.
        movementVector = new Vector2();

        // Handle the arrow keys pressed.
        if (Input.IsActionPressed("ui_up"))
        {
            movementVector += new Vector2(0, -1);
        }
        if (Input.IsActionPressed("ui_down"))
        {
            movementVector += new Vector2(0, 1);
        }
        if (Input.IsActionPressed("ui_left"))
        {
            movementVector += new Vector2(-1, 0);
        }
        if (Input.IsActionPressed("ui_right"))
        {
            movementVector += new Vector2(1, 0);
        }

        // Normalize the movement.
        movementVector = movementVector.Normalized() * delta * MoveSpeed;
    }

    // Called to apply damage to the player.
    public void ApplyDamage(float damageAmount)
    {
        this.Health -= damageAmount;

        if (this.Health <= 0)
        {
            EnvironNode2D env = this.GetParent<EnvironNode2D>();

            // Before dying, clear all references.
            foreach (ZombieKinematicBody2D zombie in env.Zombies)
            {
                if (zombie.TargetNode == this)
                {
                    zombie.TargetNode = null;
                }
            }

            this.QueueFree();
        }
    }

    public void AddToHealth(int health)
    {
        this.Health += health;
        if (this.Health > 10)
            this.Health = 10;
    }
}
