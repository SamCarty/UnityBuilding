using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldController : MonoBehaviour {
    
    // By keeping an instance of ourselves, we can ensure there is only 1 WorldController present.
    public static WorldController instance { get; private set; }
    
    // Our world that we are controlling right now.
    public World world { get; private set; }
    
    // Map containing the Tile logic elements and their corresponding GameObject in the editor.
    Dictionary<Tile, GameObject> tileGameObjectMap;
    
    // Map of all InstalledObjects logic elements and their corresponding GameObject in the editor.
    Dictionary<InstalledObject, GameObject> installedObjectGameObjectMap;

    // Start is called before the first frame update.
    void Start() {
        if (instance != null) {
            Debug.LogError("There should never be more than one WorldController!");
        }
        else {
            instance = this;
        }

        // Create world with empty tiles.
        world = new World();
        world.RegisterInstalledObjectTypeChanged(OnInstalledObjectPlaced);

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
                SpriteRenderer spriteRenderer = tileObject.AddComponent<SpriteRenderer>();
                tileData.ChangeSprite(spriteRenderer);
            }
        }

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

    public void ChangeTileType(Tile oldTile, TileType newTileType) {
        GameObject tileObject;
        if (tileGameObjectMap.TryGetValue(oldTile, out tileObject)) {
            tileGameObjectMap.Remove(oldTile);
            
            Tile newTile = oldTile.ChangeTileType(newTileType);
            world.tiles[oldTile.x, oldTile.y] = newTile;
            
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

        return world.GetTileAt(x, y);
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
        placedObject.ChangeSprite(spriteRenderer, world);

        // Register the InstalledObject changed callback.
        placedObject.RegisterChanged(OnInstalledObjectChanged);
    }

    void OnInstalledObjectChanged(InstalledObject installedObject) {
        // Make sure the InstalledObject graphics are updated when an adjacent thing is added.
        if (installedObjectGameObjectMap.TryGetValue(installedObject, out var obj)) {
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            installedObject.ChangeSprite(spriteRenderer, world); //GetSpriteForInstalledObject(installedObject);
        }
        else {
            Debug.LogError("OnInstalledObjectChanged - InstalledObject is not present in the " +
                           "installedObjectsGameObjectMap.");
        }
    }
}