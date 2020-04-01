using System;
using UnityEngine;

// The base type of a tile for example, grass or concrete.
public enum TileType {
    Empty,
    Floor
}
public class Tile {
    TileType type = TileType.Empty; // Defaults to empty

    // Callback to handle what happens when the tile type is changed.
    Action<Tile> cbTileTypeChanged;

    public TileType Type {
        get => type;
        set {
            TileType oldType = type;
            type = value;

            if (cbTileTypeChanged != null && oldType != type) cbTileTypeChanged?.Invoke(this);
        }
    }

    // An object that is 'placed' on the tile. For example, a piece of stone or a tool.
    LooseObject looseObject;

    // An object that is 'fixed' onto the tile. For example, a door or a chair.
    public InstalledObject installedObject { get; protected set; }

    // The world that owns this tile.
    public World world { get; protected set; }

    public int x;
    public int y;
    
    // Constructor
    public Tile(World world, int x, int y) {
        this.world = world;
        this.x = x;
        this.y = y;
    }
    
    public void RegisterTileTypeChanged(Action<Tile> callback) {
        cbTileTypeChanged += callback;
    }

    public void UnregisterTileTypeChanged(Action<Tile> callback) {
        cbTileTypeChanged -= callback;
    }

    /*
     * Places an InstalledObject on this Tile.
     */
    public bool PlaceObject(InstalledObject installedObject) {
        if (installedObject == null) {
            // Uninstall
            this.installedObject = null;
            return true;
        }

        if (this.installedObject != null) {
            Debug.LogError("Tile - Trying to place InstalledObject over an existing object.");
            return false;
        }

        this.installedObject = installedObject;
        return true;
    }
}