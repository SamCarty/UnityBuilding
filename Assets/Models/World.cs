using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class World {
    // Array of all the tiles in the world.
    public Tile[,] tiles { get; protected set; }

    Dictionary<InstalledObjectType, InstalledObject> installedObjectPrototypes;

    public int width;
    public int height;

    Action<InstalledObject> cbInstalledObjectTypeChanged;

    // Constructor
    public World(int width = 100, int height = 100) {
        this.width = width;
        this.height = height;

        tiles = new Tile[width + 1, height + 1];

        // Loop through for height and width to create array of tiles.
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                tiles[x, y] = new Ground(this, x, y);
            }
        }

        Debug.Log("World generated with " + tiles.Length + " tiles.");

        CreateInstalledObjectPrototypes();
    }

    void CreateInstalledObjectPrototypes() {
        installedObjectPrototypes = new Dictionary<InstalledObjectType, InstalledObject>();
        installedObjectPrototypes.Add(InstalledObjectType.Wall, new Wall());
    }

    // Randomizes the tile type of each tile in the world.
    /*
    public void RandomizeTiles() {
        Debug.Log("Randomizing tiles...");
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (Random.Range(0, 2) == 0) {
                    tiles[x, y].Type = TileType.Ground;
                }
                else {
                    tiles[x, y].Type = TileType.Floor;
                }
            }
        }
    }
    */

    // Returns the Tile object for a given coordinate.
    public Tile GetTileAt(int x, int y) {
        if (x < 0 || x > width || y < 0 || y > height) {
            Debug.LogError("Tile (" + x + ", " + y + ") is out of range.");
            return null;
        }

        return tiles[x, y];
    }

    public void PlaceInstalledObject(InstalledObjectType installedObjectType, Tile tile) {
        if (installedObjectPrototypes.TryGetValue(installedObjectType, out var installedObject)) {
            InstalledObject obj = installedObject.PlacePrototype(installedObject, tile);
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