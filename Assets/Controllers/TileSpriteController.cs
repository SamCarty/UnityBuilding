using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileSpriteController : MonoBehaviour {
    
    [SerializeField] private Sprite groundSprite;
    [SerializeField] private Sprite floorSprite;

    World world => WorldController.instance.world;

    // Map containing the Tile logic elements and their corresponding GameObject in the editor.
    Dictionary<Tile, GameObject> tileGameObjectMap;

    // Start is called before the first frame update.
    void Start() {
        // Initialize map that tracks the GameObject that is representing the Tile data.
        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        
        // Set properties of each tile.
        for (int x = 0; x < world.width; x++) {
            for (int y = 0; y < world.height; y++) {
                Tile tileData = world.GetTileAt(x, y);

                GameObject tileObject = new GameObject();
                tileObject.name = "Tile_" + x + "," + y;
                tileObject.transform.position = new Vector3(tileData.x, tileData.y);
                tileObject.transform.SetParent(transform, true);

                // Add the Tile data and GameObject to the map.
                tileGameObjectMap.Add(tileData, tileObject);
                tileObject.AddComponent<SpriteRenderer>().sprite = groundSprite;
                tileData.RegisterTileTypeChangedCallback(OnTileTypeChanged);
            }
        }

        // Register our callback events
        world.RegisterTileTypeChanged(OnTileTypeChanged);
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

                // Destroy the GameObject.
                Destroy(tileObject);
            }
            else {
                Debug.LogError(
                    "DestroyAllTileGameObjects - Tile GameObject not bound to Tile. Did you forget to bind the Tile and GameObject in the map?");
            }
        }

        // Possibly need to call another function after to reinitialise the visual representation after the level is
        // ready.
    }

    /*
     * Sets the new tile type for the given Tile.
     * Called when the tile type is changed by the corresponding tile.
     */
    void OnTileTypeChanged(Tile tileData) {
        // Try and get the tile data from the map. If it's false or null (not in the map), skip it!
        if (tileGameObjectMap.TryGetValue(tileData, out GameObject tileObject)) {
            if (tileObject == null) {
                Debug.LogError(
                    "OnTileTypeChanged - Tile GameObject is null! Did you forget to bind the Tile and GameObject in the map?");
                return;
            }

            switch (tileData.tileType) {
                case TileType.Floor:
                    tileObject.GetComponent<SpriteRenderer>().sprite = floorSprite;
                    break;
                case TileType.Ground:
                    tileObject.GetComponent<SpriteRenderer>().sprite = groundSprite;
                    break;
                default:
                    Debug.LogError("OnTileTypeChanged - Invalid tile type.");
                    break;
            }
        }
        else {
            Debug.LogError(
                "OnTileTypeChanged - Tile GameObject not bound to Tile. Did you forget to bind the Tile and GameObject in the map?");
        }
    }
}