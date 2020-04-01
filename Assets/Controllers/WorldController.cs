using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class WorldController : MonoBehaviour {

    public static WorldController Instance { get; private set; }

    Dictionary<Tile, GameObject> tileGameObjectMap;
    
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
        
        // Initialize map that tracks the GameObject that is representing the Tile data.
        tileGameObjectMap = new Dictionary<Tile, GameObject>();

        // Set properties of each tile.
        for (int x = 0; x < World.Width; x++) {
            for (int y = 0; y < World.Height; y++) {
                Tile tileData = World.GetTileAt(x, y);
                GameObject tileObject = new GameObject();
                tileObject.name = "Tile_" + x + "," + y;
                tileObject.transform.position = new Vector3(tileData.X, tileData.Y);
                tileObject.transform.SetParent(transform, true);
                
                // Add the Tile data and GameObject to the map.
                tileGameObjectMap.Add(tileData, tileObject);

                // Add a SpriteRenderer to each tile.
                tileObject.AddComponent<SpriteRenderer>();

                // Register the tile type changed callback.
                tileData.RegisterTileTypeChangedCallback(OnTileTypeChanged);
            }
        }

        World.RandomizeTiles();
    }

    /**
     * Destroys all Tile GameObject elements (not the actual Tiles).
     * Could be called when changing level to remove the visual representation of the Tiles but not the Tiles
     * themselves.
     */
    void DestroyAllTileGameObjects() {
        while (tileGameObjectMap.Count > 0) {
            Tile currentTile = tileGameObjectMap.Keys.ElementAt(0);
            
            if (tileGameObjectMap.TryGetValue(currentTile, out GameObject tileObject)) {
                // Remove from the map.
                tileGameObjectMap.Remove(currentTile);
                
                // Unregister callback.
                currentTile.UnregisterTileTypeChangedCallback(OnTileTypeChanged);
                
                // Destroy the GameObject.
                Destroy(tileObject);
            }
            else {
                Debug.LogError("DestroyAllTileGameObjects - Tile GameObject not bound to Tile. Did you forget to bind the Tile and GameObject in the map?");
            }
        }
        
        /* Possibly need to call another function after to reinitialise the visual representation after the level is
         ready.*/
    }

    /*
     * Sets the new tile type for the given Tile.
     * Called when the tile type is changed by the corresponding tile.
     */
    void OnTileTypeChanged(Tile tileData) {
        // Try and get the tile data from the map. If it's false or null (not in the map), skip it!
        if (tileGameObjectMap.TryGetValue(tileData, out GameObject tileObject)) {
            if (tileObject == null) {
                Debug.LogError("OnTileTypeChanged - Tile GameObject is null! Did you forget to bind the Tile and GameObject in the map?");
                return;
            }

            switch (tileData.Type) {
                case Tile.TileType.Floor:
                    tileObject.GetComponent<SpriteRenderer>().sprite = floorSprite;
                    break;
                case Tile.TileType.Empty:
                    tileObject.GetComponent<SpriteRenderer>().sprite = null;
                    break;
                default:
                    Debug.LogError("OnTileTypeChanged: Invalid tile type.");
                    break;
            }
        }
        else {
            Debug.LogError("OnTileTypeChanged - Tile GameObject not bound to Tile. Did you forget to bind the Tile and GameObject in the map?");
        }
    }
    
    public Tile GetTileAtCoords(Vector3 coords) {
        int x = Mathf.FloorToInt(coords.x);
        int y = Mathf.FloorToInt(coords.y);

        return World.GetTileAt(x, y);
    }
}
