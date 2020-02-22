using Godot;
using System;
using System.Collections.Generic;

public class Inventory : Panel
{
    [Signal] public delegate void CellClicked(CellGui cell);

    // Fields
    [Export] public int NumCells = 10;

    private List<CellGui> cells = new List<CellGui>();
    private PackedScene cellGuiScene;

    // Called when the inventory enters the scene tree for the first time.
    public override void _Ready()
    {
        cellGuiScene = GD.Load<PackedScene>("res://scenes/CellGui.tscn");

        for (int i = 0; i < this.NumCells; i++)
        {
            CellGui cell = (CellGui)cellGuiScene.Instance();
            this.AddChild(cell);
            cell.SetPosition(new Vector2(64 * i, 0));
            cell.Connect("CellClicked", this, "OnCellClicked");
            this.cells.Add(cell);
        }

        this.SetSize(new Vector2(64*NumCells, 64));

    }

    void OnCellClicked(CellGui cell)
    {
        // Emit Inventory's signal.
        this.EmitSignal("CellClicked", cell);
    }

    // Adds a Gun to the Inventory.
    public void AddGun(Gun gun)
    {
        foreach (CellGui cell in this.cells)
        {
            if (cell.gun == null)
            {
                cell.SetGun(gun);
                break;
            }
        }
    }
}
