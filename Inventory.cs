using Godot;
using System;

public class Inventory : Panel
{
    [Signal] public delegate void CellClicked(CellGui cell);

    // Fields
    CellGui cell1;
    CellGui cell2;
    CellGui cell3;

    // Called when the inventory enters the scene tree for the first time.
    public override void _Ready()
    {
        // Instantiate CellGuis
        cell1 = this.GetNode<CellGui>("Cell1");
        cell2 = this.GetNode<CellGui>("Cell2");
        cell3 = this.GetNode<CellGui>("Cell3");

        // Listen to CellGui singnals being clicked
        cell1.Connect("CellClicked", this, "OnCellClicked");
        cell2.Connect("CellClicked", this, "OnCellClicked");
        cell3.Connect("CellClicked", this, "OnCellClicked");
    }

    void OnCellClicked(CellGui cell)
    {
        // Emit Inventory's signal.
        this.EmitSignal("CellClicked", cell);
    }

    // Adds a Gun to the Inventory.
    public void AddGun(Gun gun)
    {
        // Do nothing if there are no empty cells in the inventory
        if (cell1.gun != null && cell2.gun != null && cell3.gun != null)
        {
            GD.Print("ERROR: AddGun() called when Inventory is already full.");
            return;
        }

        if (cell1.gun == null)
        {
            cell1.SetGun(gun);
        }
        else if (cell2.gun == null)
        {
            cell2.SetGun(gun);
        }
        else
        {
            cell3.SetGun(gun);
        }
    }
}
