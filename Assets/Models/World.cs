using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {
    // Array of all the tiles in the world.
    Tile[,] tiles;

    int width;
    public int Width => width;

    int height;
    public int Height => height;

    // Constructor
    public World(int width = 100, int height = 100) {
        this.width = width;
        this.height = height;

        tiles = new Tile[width, height];

        // Loop through for height and width to create array of tiles.
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                tiles[x, y] = new Tile(this, x, y);
            }
        }

        Debug.Log("World generated with " + tiles.Length + " tiles.");
    }

    // Randomizes the tile type of each tile in the world.
    public void RandomizeTiles() {
        Debug.Log("Randomizing tiles...");
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (Random.Range(0, 2) == 0) {
                    tiles[x, y].Type = Tile.TileType.Empty;
                }
                else {
                    tiles[x, y].Type = Tile.TileType.Floor;
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
}