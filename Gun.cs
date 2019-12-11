using Godot;
using System;

// Gun inherits from Sprite, so all guns now have Texture property.
public class Gun : Sprite
{

    public virtual void PullTrigger()
    {
        throw new NotImplementedException(); // sub classes *must* inherit
    }

    public virtual void ReleaseTrigger()
    {
        throw new NotImplementedException(); // sub classes *must* inherit
    }
}
