using System;
using UnityEngine;

// The base type of a tile for example, grass or concrete.
public enum TileType {
    Ground,
    Floor
}
public class Tile {
    
    public int x;
    public int y;
    public LooseObject looseObject { get; protected set; }
    public InstalledObject installedObject { get; protected set; }
    public World world { get; protected set; }

    private TileType tt = TileType.Ground;
    public TileType tileType {
        get => tt;
        set {
            TileType oldType = tt;
            tt = value;
            if (cbTileTypeChanged != null && oldType != tt) cbTileTypeChanged?.Invoke(this);
        }
    }

    private Action<Tile> cbTileTypeChanged;

    public Tile(World world, int x, int y) {
        this.world = world;
        this.x = x;
        this.y = y;
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

        this.installedObject = installedObject;
        return true;
    }

    public void RegisterTileTypeChangedCallback(Action<Tile> callback) {
        cbTileTypeChanged += callback;
    }

    public void UnregisterTileTypeChangedCallback(Action<Tile> callback) {
        cbTileTypeChanged -= callback;
    }

}