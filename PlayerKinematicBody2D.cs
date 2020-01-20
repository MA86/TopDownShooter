using Godot;
using System;

public class PlayerKinematicBody2D : KinematicBody2D
{
    [Export] public float MoveSpeed = 120;          // In pixels per second
    [Export] public float Health = 10;

    public Barrel OverBarrel = null;                // The barrel that the player is currently over
    public Barrel Barrel = null;                    // The barrel that the Player is currently holding (if null, he isn't holding a barrel)
    public MountableRegion MountableRegion = null;  // The mountable region that the player is currently over (doesn't mean he is mounted yet)
    public bool Mounted = false;                    // Whether the player is currently mounted or not

    private GenericGun gun;

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

        // If we are mounted, restrict angle
        if (this.Mounted)
        {
            float amount = Mathf.Deg2Rad(this.MountableRegion.Arc / 2.0f);
            if (this.Rotation > this.MountableRegion.Rotation + amount)
                this.Rotation = this.MountableRegion.Rotation + amount;
            if (this.Rotation < this.MountableRegion.Rotation - amount)
                this.Rotation = this.MountableRegion.Rotation - amount;
        }

        // Based on the arrow key input, move.
        KinematicCollision2D collisionInfo = this.MoveAndCollide(GetMovementVector(delta), false);
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

    // This method is called when there is an input event (mouse/keyboard/joystick etc) that hasn't been handled yet.
    public override void _UnhandledInput(InputEvent @event)
    {
        // mouse event
        if (@event is InputEventMouseButton mouseEvent)
        {
            // left mouse button pressed - pull gun trigger
            if ((mouseEvent.Pressed) && (mouseEvent.ButtonIndex == (int)ButtonList.Left))
            {
                if(this.gun == null)
                    return;
                
                this.gun.PullTrigger();
                GetTree().SetInputAsHandled();
                return;
            }

            // left mouse button released - release gun trigger
            if ((!mouseEvent.Pressed) && (mouseEvent.ButtonIndex == (int)ButtonList.Left))
            {
                if (this.gun == null)
                    return;

                this.gun.ReleaseTrigger();
                GetTree().SetInputAsHandled();
                return;
            }

            // mouse wheel - zoom in/out
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

        // key event
        if (@event is InputEventKey asKeyEvent)
        {
            // e pressed event
            if ((asKeyEvent.Pressed) && (asKeyEvent.Scancode == (int)KeyList.E))
            {
                // if over barrel, pick it up
                if (this.OverBarrel != null && this.Barrel == null)
                {
                    this.Barrel = this.OverBarrel;
                    this.Barrel.Position = new Vector2(0, 0); // place barrel at (0,0) relative to the player
                    this.Barrel.GetParent().RemoveChild(this.Barrel); // remove barrel from its current parent
                    this.AddChild(this.Barrel); // add it as a child to player
                    this.GetTree().SetInputAsHandled();
                    return;
                }

                // if holding barrel, drop it
                if (this.Barrel != null)
                {
                    this.RemoveChild(this.Barrel);
                    this.GetNode<Node2D>("/root/EnvironNode2D/OnGround").AddChild(this.Barrel);
                    this.Barrel.Position = this.Position;
                    this.Barrel = null;
                    this.GetTree().SetInputAsHandled();
                    return;
                }

                // if in mountable region, mount
                if (!Mounted && (MountableRegion != null))
                {
                    this.Mounted = true;
                    this.Position = MountableRegion.Position;
                    this.Rotation = MountableRegion.Rotation;
                    this.MoveSpeed = 0;
                    if (this.gun != null)
                        this.gun.Mounted = true;
                    GetTree().SetInputAsHandled();
                    return;
                }

                // if mounted, unmount
                if (Mounted)
                {
                    this.Mounted = false;
                    this.MoveSpeed = 120;
                    if (this.gun != null)
                        this.gun.Mounted = false;
                    GetTree().SetInputAsHandled();
                    return;
                }
            }
        }
    }

    // Calculate a movement vector based on which arrow keys are pressed and how much time since last frame.
    public Vector2 GetMovementVector(float delta)
    {
        Vector2 movementVector = new Vector2();

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

        // scale by speed 
        movementVector = movementVector.Normalized() * delta * MoveSpeed;
        return movementVector;
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
