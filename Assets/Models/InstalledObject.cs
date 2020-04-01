using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Things like walls, doors and furniture!
 */
public enum InstalledObjectType {
    Wall
}

public class InstalledObject {
    // The base Tile of the InstalledObject. Large objects can occupy multiple Tiles.
    public Tile tile { get; protected set; }
    public InstalledObjectType installedObjectType { get; protected set; }

    //A multiplier that determines the speed of movement. A value of '2' means you go twice as slow (half speed).
    // If this is 0, the Tile is impassable! 
    float movementCost = 1f;

    int width = 1;
    int height = 1;

    Action<InstalledObject> cbChanged;

    protected InstalledObject() {
    }

    public static InstalledObject CreatePrototype(InstalledObjectType installedObjectType, float movementCost, 
        int width = 1, int height = 1) {
        InstalledObject installedObject = new InstalledObject();
        installedObject.installedObjectType = installedObjectType;
        installedObject.width = width;
        installedObject.height = height;
        installedObject.movementCost = movementCost;
        return installedObject;
    }

    public static InstalledObject PlacePrototype(InstalledObject proto, Tile tile) {
        InstalledObject installedObject = new InstalledObject();
        installedObject.installedObjectType = proto.installedObjectType;
        installedObject.width = proto.width;
        installedObject.height = proto.height;
        installedObject.movementCost = proto.movementCost;
        installedObject.tile = tile;
        
        // TODO: This assumes that the object is a 1x1 object.
        if (installedObject.tile.PlaceObject(installedObject) == false) {
            // If we can't place object here for some reason.
            return null;
        }

        return installedObject;
    }

    public void RegisterChanged(Action<InstalledObject> callback) {
        cbChanged += callback;
    }

    public void UnregisterChanged(Action<InstalledObject> callback) {
        cbChanged -= callback;
    }
    
    
}