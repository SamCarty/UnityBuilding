using System;
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
    
    // Size of the InstalledObject
    private int width = 1;
    private int height = 1;

    // A multiplier that determines the speed of movement. A value of '2' means you go twice as slow (half speed).
    // If this is 0, the Tile is impassable! 
    private float movementCost = 1f;

    // Whether the InstalledObject connects to its neighbour (e.g. a wall for it's sprite to change.
    public bool linksToNeighbour { get; protected set; }
    
    // Whether the InstalledObject is actually installed on a Tile (or is queued up to be installed!).
    public bool installed { get; protected set; }

    // Callback for when the status changes.
    Action<InstalledObject> cbChanged;

    public static InstalledObject CreatePrototype(InstalledObjectType installedObjectType, float movementCost,
        bool linksToNeighbour = false, int width = 1, int height = 1, bool installed = false) {
        InstalledObject installedObject = new InstalledObject();
        installedObject.installedObjectType = installedObjectType;
        installedObject.linksToNeighbour = linksToNeighbour;
        installedObject.width = width;
        installedObject.height = height;
        installedObject.movementCost = movementCost;
        installedObject.installed = installed;
        return installedObject;
    }

    public static InstalledObject PlacePrototype(InstalledObject proto, Tile tile) {
        if (!CheckPlacementValidity(proto.installedObjectType, tile)) {
            return null;
        }
        
        InstalledObject installedObject = new InstalledObject();
        installedObject.installedObjectType = proto.installedObjectType;
        installedObject.linksToNeighbour = proto.linksToNeighbour;
        installedObject.width = proto.width;
        installedObject.height = proto.height;
        installedObject.movementCost = proto.movementCost;
        installedObject.installed = proto.installed;
        installedObject.tile = tile;

        // TODO: This assumes that the object is a 1x1 object.
        if (installedObject.tile.PlaceObject(installedObject) == false) {
            // If we can't place object here for some reason.
            return null;
        }

        tile.pendingInstalledObjectJob = null; // Get rid of the job.
        InformNeighbours(installedObject);

        return installedObject;
    }

    // TODO: maybe move these methods to some static class?
    public static bool CheckPlacementValidity(InstalledObjectType type, Tile tile) {
        // TODO support different InstallObjectTypes
        if (tile.tileType != TileType.Floor) {
            Debug.LogError("CheckPlacementValidity - Tried to place InstalledObject on area with no foundation.");
            return false;
        }
        
        if (tile.installedObject != null) {
            Debug.LogError("CheckPlacementValidity - Tried to place InstalledObject over an existing object.");
            return false;
        }

        if (tile.pendingInstalledObjectJob != null) {
            Debug.LogError("CheckPlacementValidity - Tried to place InstalledObject over an a queued job.");
            return false;
        }

        return true;
    }

    public static void InformNeighbours(InstalledObject installedObject) {
        if (installedObject.linksToNeighbour) {
            // Inform the neighbours that we have been added so they can update their own graphics.
            // Trigger the OnInstalledObjectChanged callback.
            Tile t;
            int x = installedObject.tile.x;
            int y = installedObject.tile.y;

            t = installedObject.tile.world.GetTileAt(x, y + 1);
            if (t != null && t.installedObject != null &&
                t.installedObject.installedObjectType == installedObject.installedObjectType) {
                t.installedObject.cbChanged(t.installedObject);
            }

            t = installedObject.tile.world.GetTileAt(x, y - 1);
            if (t != null && t.installedObject != null &&
                t.installedObject.installedObjectType == installedObject.installedObjectType) {
                t.installedObject.cbChanged(t.installedObject);
            }

            t = installedObject.tile.world.GetTileAt(x + 1, y);
            if (t != null && t.installedObject != null &&
                t.installedObject.installedObjectType == installedObject.installedObjectType) {
                t.installedObject.cbChanged(t.installedObject);
            }

            t = installedObject.tile.world.GetTileAt(x - 1, y);
            if (t != null && t.installedObject != null &&
                t.installedObject.installedObjectType == installedObject.installedObjectType) {
                t.installedObject.cbChanged(t.installedObject);
            }
        }
    }

    public void SetInstalled(bool installed) {
        this.installed = installed;
        if (cbChanged != null) cbChanged?.Invoke(this);
    }
    
    public void RegisterChanged(Action<InstalledObject> callback) {
        cbChanged += callback;
    }

    public void UnregisterChanged(Action<InstalledObject> callback) {
        cbChanged -= callback;
    }
}