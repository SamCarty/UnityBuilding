using System;
using System.Collections.Generic;
using UnityEngine;

public class World {
    // Array of all the tiles in the world.
    public Tile[,] tiles { get; protected set; }

    Dictionary<InstalledObjectType, InstalledObject> installedObjectPrototypes;

    public int width;
    public int height;

    Action<InstalledObject> cbInstalledObjectPlaced;
    Action<Tile> cbTileTypeChanged;

    public JobQueue jobQueue;

    // Constructor
    public World(int width = 100, int height = 100) {
        this.width = width;
        this.height = height;

        tiles = new Tile[width + 1, height + 1];

        // Loop through for height and width to create array of tiles.
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                tiles[x, y] = new Tile(this, x, y);
                tiles[x, y].RegisterTileTypeChangedCallback(OnTileTypeChanged);
            }
        }

        Debug.Log("World generated with " + tiles.Length + " tiles.");

        jobQueue = new JobQueue();

        CreateInstalledObjectPrototypes();
    }

    void CreateInstalledObjectPrototypes() {
        installedObjectPrototypes = new Dictionary<InstalledObjectType, InstalledObject>();
        installedObjectPrototypes.Add(InstalledObjectType.Wall, InstalledObject.CreatePrototype(
            InstalledObjectType.Wall, 0, true, width: 1, height: 1));
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
            InstalledObject obj = InstalledObject.PlacePrototype(installedObject, tile);
            if (obj != null) {
                cbInstalledObjectPlaced?.Invoke(obj);
            }
        }
        else {
            Debug.LogError("World - installedObjectPrototypes does not contain the prototype Installed Object" +
                           " for " + installedObjectType);
        }
    }

    public void RegisterInstalledObjectPlaced(Action<InstalledObject> callback) {
        cbInstalledObjectPlaced += callback;
    }

    public void UnregisterInstalledObjectPlaced(Action<InstalledObject> callback) {
        cbInstalledObjectPlaced -= callback;
    }
    
    public void RegisterTileTypeChanged(Action<Tile> callback) {
        cbTileTypeChanged += callback;
    }

    public void UnregisterTileTypeChanged(Action<Tile> callback) {
        cbTileTypeChanged -= callback;
    }
    
    private void OnTileTypeChanged(Tile tile) {
        cbTileTypeChanged?.Invoke(tile);
    }

}