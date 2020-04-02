using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldController : MonoBehaviour {
    
    // By keeping an instance of ourselves, we can ensure there is only 1 WorldController present.
    public static WorldController instance { get; private set; }
    
    // Our world that we are controlling right now.
    public World world { get; private set; }

    [SerializeField]
    private Sprite groundSprite;
    [SerializeField]
    private Sprite floorSprite;

    // Map containing the Tile logic elements and their corresponding GameObject in the editor.
    Dictionary<Tile, GameObject> tileGameObjectMap;
    
    // Map of all InstalledObjects logic elements and their corresponding GameObject in the editor.
    Dictionary<InstalledObject, GameObject> installedObjectGameObjectMap;
    
    Dictionary<string, Sprite> installedObjectSprites;

    // Start is called before the first frame update.
    void Start() {
        installedObjectSprites = new Dictionary<string, Sprite>();
        
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/InstalledObjects/");
        foreach (Sprite sprite in sprites) {
            installedObjectSprites.Add(sprite.name, sprite);
        }
        
        if (instance != null) {
            Debug.LogError("There should never be more than one WorldController!");
        }
        else {
            instance = this;
        }

        // Create world with empty tiles.
        world = new World();
        world.RegisterInstalledObjectPlaced(OnInstalledObjectPlaced);

        // Initialize map that tracks the GameObject that is representing the Tile data.
        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        installedObjectGameObjectMap = new Dictionary<InstalledObject, GameObject>();

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

        // Position the camera in the centre of the world.
        if (Camera.main != null) {
            var cameraTransform = Camera.main.transform;
            cameraTransform.position = new Vector3(world.width / 2, world.height / 2,
                cameraTransform.position.z);
        }

        //World.RandomizeTiles();
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

    public Tile GetTileAtCoords(Vector3 coords) {
        int x = Mathf.FloorToInt(coords.x);
        int y = Mathf.FloorToInt(coords.y);

        return world.GetTileAt(x, y);
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

    public void OnInstalledObjectPlaced(InstalledObject placedObject) {
        // Create the visual representation (GameObject) of the InstalledObject.
        // TODO: Does not work with objects larger than 1 tile right now or rotated objects!

        GameObject installedObjectGameObject = new GameObject();
        installedObjectGameObject.name =
            placedObject.installedObjectType + "_" + placedObject.tile.x + "," + placedObject.tile.y;
        installedObjectGameObject.transform.position = new Vector3(placedObject.tile.x, placedObject.tile.y);
        installedObjectGameObject.transform.SetParent(transform, true);

        // Add the InstalledObject data and GameObject to the map.
        installedObjectGameObjectMap.Add(placedObject, installedObjectGameObject);

        // Add a SpriteRenderer to the InstalledObject.
        installedObjectGameObject.AddComponent<SpriteRenderer>();
        switch (placedObject.installedObjectType) {
            case InstalledObjectType.Wall:
                SpriteRenderer spriteRenderer = installedObjectGameObject.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = GetSpriteForInstalledObject(placedObject);
                spriteRenderer.sortingLayerName = "InstalledObject";
                break;
            default:
                Debug.LogError("OnInstalledObjectPlaced - Invalid InstalledObject type.");
                break;
        }

        // Register the InstalledObject changed callback.
        placedObject.RegisterChanged(OnInstalledObjectChanged);
    }

    Sprite GetSpriteForInstalledObject(InstalledObject obj) {
        // if the object does not link to the neighbour, just get the sprite by the ObjectType...
        if (obj.linksToNeighbour == false) {
            return installedObjectSprites[obj.installedObjectType.ToString()];
        }

        // ...otherwise, the sprite will have a more complex name!
        string spriteName = obj.installedObjectType + "_";
        int x = obj.tile.x;
        int y = obj.tile.y;

        // Check the neighbouring tiles and append the 
        Tile t;
        t = world.GetTileAt(x, y + 1);
        if (t != null && t.installedObject != null &&
            t.installedObject.installedObjectType == obj.installedObjectType) {
            spriteName += "N";
        }

        t = world.GetTileAt(x, y - 1);
        if (t != null && t.installedObject != null &&
            t.installedObject.installedObjectType == obj.installedObjectType) {
            spriteName += "S";
        }

        t = world.GetTileAt(x + 1, y);
        if (t != null && t.installedObject != null &&
            t.installedObject.installedObjectType == obj.installedObjectType) {
            spriteName += "E";
        }

        t = world.GetTileAt(x - 1, y);
        if (t != null && t.installedObject != null &&
            t.installedObject.installedObjectType == obj.installedObjectType) {
            spriteName += "W";
        }

        if (installedObjectSprites.TryGetValue(spriteName, out var sprite)) {
            return sprite;
        }
        else {
            Debug.LogError("GetSpriteForInstalledObject - Sprite is not present in the installedObjectSprites " +
                           "Map");
            // TODO: return a default sprite
            return null;
        }
    }

    void OnInstalledObjectChanged(InstalledObject installedObject) {
        // Make sure the InstalledObject graphics are updated when an adjacent thing is added.
        if (installedObjectGameObjectMap.TryGetValue(installedObject, out var obj)) {
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = GetSpriteForInstalledObject(installedObject);
            spriteRenderer.sortingLayerName = "InstalledObject";
            
        }
        else {
            Debug.LogError("OnInstalledObjectChanged - InstalledObject is not present in the " +
                           "installedObjectsGameObjectMap.");
        }
    }
}