﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class World {
    // Array of all the tiles in the world.
    Tile[,] tiles;

    Dictionary<InstalledObjectType, InstalledObject> installedObjectPrototypes;

    int width;
    public int Width => width;

    int height;
    public int Height => height;

    Action<InstalledObject> cbInstalledObjectTypeChanged;

    // Constructor
    public World(int width = 100, int height = 100) {
        this.width = width;
        this.height = height;

        tiles = new Tile[width + 1, height + 1];

        // Loop through for height and width to create array of tiles.
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                tiles[x, y] = new Tile(this, x, y);
            }
        }

        Debug.Log("World generated with " + tiles.Length + " tiles.");

        CreateInstalledObjectPrototypes();
    }

    void CreateInstalledObjectPrototypes() {
        installedObjectPrototypes = new Dictionary<InstalledObjectType, InstalledObject>();
        installedObjectPrototypes.Add(InstalledObjectType.Wall, InstalledObject.CreatePrototype(
            InstalledObjectType.Wall, 
            0, width: 1, height: 1));
    }

    // Randomizes the tile type of each tile in the world.
    public void RandomizeTiles() {
        Debug.Log("Randomizing tiles...");
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (Random.Range(0, 2) == 0) {
                    tiles[x, y].Type = TileType.Empty;
                }
                else {
                    tiles[x, y].Type = TileType.Floor;
                }
            }
        }
    }

    // Returns the Tile object for a given coordinate.
    public Tile GetTileAt(int x, int y) {
        if (x < 0 || x > Width || y < 0 || y > Height) {
            Debug.LogError("Tile (" + x + ", " + y + ") is out of range.");
            return null;
        }

        return tiles[x, y];
    }
    
    public void PlaceInstalledObject(InstalledObjectType installedObjectType, Tile tile) {
        if (installedObjectPrototypes.TryGetValue(installedObjectType, out var installedObject)) {
            InstalledObject obj = InstalledObject.PlacePrototype(installedObject, tile);
            if (obj != null) {
                cbInstalledObjectTypeChanged?.Invoke(obj);
            }
        }
        else {
            Debug.LogError("World - installedObjectPrototypes does not contain the prototype Installed Object" +
                           " for " + installedObjectType);
        }
    }

    public void RegisterInstalledObjectTypeChanged(Action<InstalledObject> callback) {
        cbInstalledObjectTypeChanged += callback;
    }

    public void UnregisterInstalledObjectTypeChanged(Action<InstalledObject> callback) {
        cbInstalledObjectTypeChanged -= callback;
    }

}