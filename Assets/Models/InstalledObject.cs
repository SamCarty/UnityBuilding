﻿using System;
using System.Collections;
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

// Size of the InstalledObject
    int width = 1;
    int height = 1;

    // A multiplier that determines the speed of movement. A value of '2' means you go twice as slow (half speed).
    // If this is 0, the Tile is impassable! 
    float movementCost = 1f;

    // Whether the InstalledObject connects to its neighbour (e.g. a wall for it's sprite to change.
    public bool linksToNeighbour { get; protected set; }


    // Callback for when the status changes.
    Action<InstalledObject> cbChanged;

    protected InstalledObject() {
    }

    protected InstalledObject CreatePrototype(InstalledObject installedObject, float movementCost,
        bool linksToNeighbour, int width, int height) {
        installedObject.installedObjectType = installedObjectType;
        installedObject.linksToNeighbour = linksToNeighbour;
        installedObject.width = width;
        installedObject.height = height;
        installedObject.movementCost = movementCost;
        return installedObject;
    }

    public static InstalledObject PlacePrototype(InstalledObject proto, Tile tile) {
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
            
            Debug.Log("Coords are: " + x + ", " +  y);

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

    public void RegisterChanged(Action<InstalledObject> callback) {
        cbChanged += callback;
    }

    public void UnregisterChanged(Action<InstalledObject> callback) {
        cbChanged -= callback;
    }
}