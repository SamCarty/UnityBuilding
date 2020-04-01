using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldController : MonoBehaviour {

    public Sprite wallSprite; // TODO: make dynamic based on the object needed!
    public Sprite floorSprite; // TODO: make dynamic based on the object needed!
    
    public static WorldController Instance { get; private set; }

    Dictionary<Tile, GameObject> tileGameObjectMap;
    Dictionary<InstalledObject, GameObject> installedObjectGameObjectMap;
    
    public World World { get; private set; }
    

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
        World.RegisterInstalledObjectTypeChanged(OnInstalledObjectPlaced);
        
        // Initialize map that tracks the GameObject that is representing the Tile data.
        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        installedObjectGameObjectMap = new Dictionary<InstalledObject, GameObject>();

        // Set properties of each tile.
        for (int x = 0; x < World.Width; x++) {
            for (int y = 0; y < World.Height; y++) {
                Tile tileData = World.GetTileAt(x, y);
                
                GameObject tileObject = new GameObject();
                tileObject.name = "Tile_" + x + "," + y;
                tileObject.transform.position = new Vector3(tileData.x, tileData.y);
                tileObject.transform.SetParent(transform, true);
                
                // Add the Tile data and GameObject to the map.
                tileGameObjectMap.Add(tileData, tileObject);

                // Add a SpriteRenderer to each tile.
                tileObject.AddComponent<SpriteRenderer>();

                // Register the tile type changed callback.
                tileData.RegisterTileTypeChanged(OnTileTypeChanged);
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
                currentTile.UnregisterTileTypeChanged(OnTileTypeChanged);
                
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
                Debug.LogError(
                    "OnTileTypeChanged - Tile GameObject is null! Did you forget to bind the Tile and GameObject in the map?");
                return;
            }

            switch (tileData.Type) {
                case TileType.Floor:
                    tileObject.GetComponent<SpriteRenderer>().sprite = floorSprite;
                    break;
                case TileType.Empty:
                    tileObject.GetComponent<SpriteRenderer>().sprite = null;
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

    public Tile GetTileAtCoords(Vector3 coords) {
        int x = Mathf.FloorToInt(coords.x);
        int y = Mathf.FloorToInt(coords.y);

        return World.GetTileAt(x, y);
    }

    public void OnInstalledObjectPlaced(InstalledObject placedObject) {
        // Create the visual representation (GameObject) of the InstalledObject.
        // TODO: Does not work with objects larger than 1 tile right now or rotated objects!
        
        GameObject installedObjectGameObject = new GameObject();
        installedObjectGameObject.name = placedObject.installedObjectType + "_" + placedObject.tile.x + "," + placedObject.tile.y;
        installedObjectGameObject.transform.position = new Vector3(placedObject.tile.x, placedObject.tile.y);
        installedObjectGameObject.transform.SetParent(transform, true);
                
        // Add the InstalledObject data and GameObject to the map.
        installedObjectGameObjectMap.Add(placedObject, installedObjectGameObject);

        // Add a SpriteRenderer to the InstalledObject.
        installedObjectGameObject.AddComponent<SpriteRenderer>();
        switch (placedObject.installedObjectType) {
            case InstalledObjectType.Wall:
                installedObjectGameObject.GetComponent<SpriteRenderer>().sprite = wallSprite;
                break;
            default:
                Debug.LogError("OnInstalledObjectPlaced - Invalid InstalledObject type.");
                break;
        }

        // Register the InstalledObject changed callback.
        placedObject.RegisterChanged(OnInstalledObjectChanged);
    }

    void OnInstalledObjectChanged(InstalledObject installedObject) {
        Debug.LogError("OnInstalledObjectPlaced - Not implemented!");
    }
}
