using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldController : MonoBehaviour {
    public static WorldController Instance { get; private set; }

    Dictionary<Tile, GameObject> tileGameObjectMap;
    Dictionary<InstalledObject, GameObject> installedObjectGameObjectMap;

    Dictionary<string, Sprite> installedObjectSprites;

    public World World { get; private set; }

    // Start is called before the first frame update.
    void Start() {
        installedObjectSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/InstalledObjects/");
        foreach (Sprite sprite in sprites) {
            installedObjectSprites.Add(sprite.name, sprite);
        }

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
        for (int x = 0; x < World.width; x++) {
            for (int y = 0; y < World.height; y++) {
                Tile tileData = World.GetTileAt(x, y);

                GameObject tileObject = new GameObject();
                tileObject.name = "Tile_" + x + "," + y;
                tileObject.transform.position = new Vector3(tileData.x, tileData.y);
                tileObject.transform.SetParent(transform, true);

                // Add the Tile data and GameObject to the map.
                tileGameObjectMap.Add(tileData, tileObject);
                SpriteRenderer spriteRenderer = tileObject.AddComponent<SpriteRenderer>();
                tileData.ChangeSprite(spriteRenderer);
            }
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

    public void ChangeTileType(Tile oldTile, TileType newTileType) {
        GameObject tileObject;
        if (tileGameObjectMap.TryGetValue(oldTile, out tileObject)) {
            tileGameObjectMap.Remove(oldTile);
            
            Tile newTile = oldTile.ChangeTileType(newTileType);
            World.tiles[oldTile.x, oldTile.y] = newTile;
            
            // Add the updated Tile to the Map.
            tileGameObjectMap.Add(newTile, tileObject);
            
            // Change the sprite
            SpriteRenderer spriteRenderer = tileObject.GetComponent<SpriteRenderer>();
            newTile.ChangeSprite(spriteRenderer);
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
        installedObjectGameObject.name =
            placedObject.installedObjectType + "_" + placedObject.tile.x + "," + placedObject.tile.y;
        installedObjectGameObject.transform.position = new Vector3(placedObject.tile.x, placedObject.tile.y);
        installedObjectGameObject.transform.SetParent(transform, true);

        // Add the InstalledObject data and GameObject to the map.
        installedObjectGameObjectMap.Add(placedObject, installedObjectGameObject);

        // Add a SpriteRenderer to the InstalledObject.
        SpriteRenderer spriteRenderer = installedObjectGameObject.AddComponent<SpriteRenderer>();
        switch (placedObject.installedObjectType) {
            case InstalledObjectType.Wall:
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

    void OnInstalledObjectChanged(InstalledObject installedObject) {
        // Make sure the InstalledObject graphics are updated when an adjacent thing is added.
        if (installedObjectGameObjectMap.TryGetValue(installedObject, out var obj)) {
            obj.GetComponent<SpriteRenderer>().sprite = GetSpriteForInstalledObject(installedObject);
        }
        else {
            Debug.LogError("OnInstalledObjectChanged - InstalledObject is not present in the " +
                           "installedObjectsGameObjectMap.");
        }
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
        t = World.GetTileAt(x, y + 1);
        if (t != null && t.installedObject != null &&
            t.installedObject.installedObjectType == obj.installedObjectType) {
            spriteName += "N";
        }

        t = World.GetTileAt(x, y - 1);
        if (t != null && t.installedObject != null &&
            t.installedObject.installedObjectType == obj.installedObjectType) {
            spriteName += "S";
        }

        t = World.GetTileAt(x + 1, y);
        if (t != null && t.installedObject != null &&
            t.installedObject.installedObjectType == obj.installedObjectType) {
            spriteName += "E";
        }

        t = World.GetTileAt(x - 1, y);
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
}