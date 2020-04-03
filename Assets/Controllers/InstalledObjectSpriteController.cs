using System.Collections.Generic;
using UnityEngine;

public class InstalledObjectSpriteController : MonoBehaviour {

    World world => WorldController.instance.world;

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

        // Initialize map that tracks the GameObject that is representing the InstalledObject data.
        installedObjectGameObjectMap = new Dictionary<InstalledObject, GameObject>();
        
        // Register our callback events
        world.RegisterInstalledObjectPlaced(OnInstalledObjectPlaced);
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
        if (!placedObject.installed) {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);
        }
        else {
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        }

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

    public Sprite GetSpriteForInstalledObject(InstalledObject obj) {
        // if the object does not link to the neighbour, just get the sprite by the ObjectType...
        if (obj.linksToNeighbour == false) {
            return installedObjectSprites[obj.installedObjectType.ToString()];
        }

        // ...otherwise, the sprite will have a more complex name!
        string spriteName = obj.installedObjectType + "_";

        if (obj.installed == false) {
            Debug.Log("Object not yet installed...");
            // TODO remove code duplication
            if (installedObjectSprites.TryGetValue(spriteName, out var s)) {
                return s;
            }
        }
        
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
        if (!installedObject.installed) return;
        
        if (installedObjectGameObjectMap.TryGetValue(installedObject, out var obj)) {
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            
            // TODO remove duplication with the OnInstalledObjectPlaced method.
            if (!installedObject.installed) {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);
            }
            else {
                spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            }
            
            spriteRenderer.sprite = GetSpriteForInstalledObject(installedObject);
            spriteRenderer.sortingLayerName = "InstalledObject";
        }
        else {
            Debug.LogError("OnInstalledObjectChanged - InstalledObject is not present in the " +
                           "installedObjectsGameObjectMap.");
        }
    }
}