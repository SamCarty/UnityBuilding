using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * Things like walls, doors and furniture!
 */
public enum InstalledObjectType {
    Wall
}

public abstract class InstalledObject {
    // The base Tile of the InstalledObject. Large objects can occupy multiple Tiles.
    public Tile tile { get; protected set; }

    public InstalledObjectType installedObjectType { get; protected set; }
    
    protected Dictionary<string, Sprite> objectSprites = new Dictionary<string, Sprite>();

    // Size of the InstalledObject
    private int width = 1;
    private int height = 1;

    // A multiplier that determines the speed of movement. A value of '2' means you go twice as slow (half speed).
    // If this is 0, the Tile is impassable! 
    private float movementCost = 1f;

    // Whether the InstalledObject connects to its neighbour (e.g. a wall for it's sprite to change.
    public bool linksToNeighbour { get; protected set; }


    // Callback for when the status changes.
    Action<InstalledObject> cbChanged;

    protected InstalledObject CreatePrototype(InstalledObject installedObject, float movementCost,
        bool linksToNeighbour, int width, int height) {
        installedObject.installedObjectType = installedObjectType;
        installedObject.linksToNeighbour = linksToNeighbour;
        installedObject.width = width;
        installedObject.height = height;
        installedObject.movementCost = movementCost;
        return installedObject;
    }

    public InstalledObject PlacePrototype(InstalledObject proto, Tile tile) {
        if (!CheckPlacementValidity(tile)) {
            return null;
        }
        
        InstalledObject installedObject = proto.GenerateNewInstalledObject();
        installedObject.installedObjectType = proto.installedObjectType;
        installedObject.linksToNeighbour = proto.linksToNeighbour;
        installedObject.width = proto.width;
        installedObject.height = proto.height;
        installedObject.movementCost = proto.movementCost;
        installedObject.tile = tile;

        // TODO: This assumes that the object is a 1x1 object.
        if (installedObject.tile.PlaceObject(installedObject) == false) {
            // If we can't place object here for some reason.
            return null;
        }

        if (installedObject.linksToNeighbour) {
            // Inform the neighbours that we have been added so they can update their own graphics.
            // Trigger the OnInstalledObjectChanged callback.
            Tile t;
            int x = tile.x;
            int y = tile.y;

            t = tile.world.GetTileAt(x, y + 1);
            if (t != null && t.installedObject != null &&
                t.installedObject.installedObjectType == installedObject.installedObjectType) {
                t.installedObject.cbChanged(t.installedObject);
            }

            t = tile.world.GetTileAt(x, y - 1);
            if (t != null && t.installedObject != null &&
                t.installedObject.installedObjectType == installedObject.installedObjectType) {
                t.installedObject.cbChanged(t.installedObject);
            }

            t = tile.world.GetTileAt(x + 1, y);
            if (t != null && t.installedObject != null &&
                t.installedObject.installedObjectType == installedObject.installedObjectType) {
                t.installedObject.cbChanged(t.installedObject);
            }

            t = tile.world.GetTileAt(x - 1, y);
            if (t != null && t.installedObject != null &&
                t.installedObject.installedObjectType == installedObject.installedObjectType) {
                t.installedObject.cbChanged(t.installedObject);
            }
        }

        return installedObject;
    }

    protected abstract InstalledObject GenerateNewInstalledObject();

    public bool CheckPlacementValidity(Tile tile) {
        if (!tile.IsBuildonable()) {
            Debug.LogError("CheckPlacementValidity - Tried to place InstalledObject on area with no foundation.");
            return false;
        }

        if (tile.installedObject != null) {
            Debug.LogError("CheckPlacementValidity - Tried to place InstalledObject over an existing object.");
            return false;
        }

        return true;
    }
    
    public abstract void ChangeSprite(SpriteRenderer spriteRenderer, World world);

    protected Sprite GetSpriteForInstalledObject(World world, InstalledObject obj) {
        // if the object does not link to the neighbour, just get the sprite by the ObjectType...
        if (obj.linksToNeighbour == false) {
            return objectSprites[obj.installedObjectType.ToString()];
        }

        // ...otherwise, the sprite will have a more complex name!
        string spriteName = obj.installedObjectType + "_";
        int x = obj.tile.x;
        int y = obj.tile.y;

        // Check the neighbouring tiles and append the corresponding NSEW value.
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

        if (objectSprites.TryGetValue(spriteName, out var sprite)) {
            return sprite;
        }

        Debug.LogError("GetSpriteForInstalledObject - Sprite is not present in the " +
                       "installedObjectSprites map");
        return objectSprites["Wall_"];
    }
    
    public void RegisterChanged(Action<InstalledObject> callback) {
        cbChanged += callback;
    }

    public void UnregisterChanged(Action<InstalledObject> callback) {
        cbChanged -= callback;
    }
}