using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile {
    //The base type of a tile for example, grass or concrete.
    public enum TileType {
        Empty,
        Floor
    }

    TileType type = TileType.Empty; // Defaults to empty

    // Callback to handle what happens when the tile type is changed.
    Action<Tile> tileTypeChangedCallback;

    public TileType Type {
        get => type;
        set {
            TileType oldType = type;
            type = value;

            if (tileTypeChangedCallback != null && oldType != type) tileTypeChangedCallback?.Invoke(this);
        }
    }

    // An object that is 'placed' on the tile. For example, a piece of stone or a tool.
    LooseObject looseObject;

    // An object that is 'fixed' onto the tile. For example, a door or a chair.
    InstalledObject installedObject;

    // The world that owns this tile.
    World world;

    int x;
    public int X => x;

    int y;
    public int Y => y;

    // Constructor
    public Tile(World world, int x, int y) {
        this.world = world;
        this.x = x;
        this.y = y;
    }

    public void RegisterTileTypeChangedCallback(Action<Tile> callback) {
        tileTypeChangedCallback += callback;
    }

    public void UnRegisterTileTypeChangedCallback(Action<Tile> callback) {
        tileTypeChangedCallback -= callback;
    }
}