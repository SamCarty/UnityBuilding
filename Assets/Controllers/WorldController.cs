using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    public static WorldController Instance { get; private set; }
    public World World { get; private set; }
    public Sprite floorSprite;

    // Start is called before the first frame update.
    void Start() {
        if (Instance != null) {
            Debug.LogError("There should never be more than one WorldControllers!");
        }
        else {
            Instance = this;
        }
        
        // Create world with empty tiles.
        World = new World();

        // Set properties of each tile.
        for (int x = 0; x < World.Width; x++) {
            for (int y = 0; y < World.Height; y++) {
                GameObject tileObject = new GameObject();
                Tile tileData = World.GetTileAt(x, y);
                tileObject.name = "Tile_" + x + "," + y;
                tileObject.transform.position = new Vector3(tileData.X, tileData.Y);
                tileObject.transform.SetParent(transform, true);

                // Add a SpriteRenderer to each tile.
                tileObject.AddComponent<SpriteRenderer>();

                // Register the tile type changed callback.
                tileData.RegisterTileTypeChangedCallback(tile => OnTileTypeChanged(tile, tileObject));
            }
        }

        World.RandomizeTiles();
    }

    // Called when the tile type is changed by the corresponding tile.
    void OnTileTypeChanged(Tile tileData, GameObject tileObject) {
        if (tileData.Type == Tile.TileType.Floor) {
            tileObject.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }
        else if (tileData.Type == Tile.TileType.Empty) {
            tileObject.GetComponent<SpriteRenderer>().sprite = null;
        }
        else {
            Debug.LogError("OnTileTypeChanged: Invalid tile type.");
        }
    }
}