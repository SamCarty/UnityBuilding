using System;
using UnityEngine;

// The base type of a tile for example, grass or concrete.
public enum TileType {
    Ground,
    Floor
}
public abstract class Tile {
    
    public int x;
    public int y;
    public LooseObject looseObject { get; protected set; }
    public InstalledObject installedObject { get; protected set; }
    public World world { get; protected set; }
    
    protected Sprite sprite;

    protected void CreateTile(World world, int x, int y, Sprite sprite) {
        this.world = world;
        this.x = x;
        this.y = y;
        this.sprite = sprite;
    }

    public Tile ChangeTileType(TileType tileType) {
        Tile newTile;
        switch (tileType) {
            case TileType.Floor:
                newTile =  new Floor(this);
                break;
            case TileType.Ground:
                newTile = new Ground(this);
                break;
            default:
                newTile = new Ground(this);
                break;
        }
        return newTile;
    }

    /*
     * Places an InstalledObject on this Tile.
     */
    public bool PlaceObject(InstalledObject installedObject) {
        if (installedObject == null) {
            // Uninstall
            this.installedObject = null;
            return true;
        }

        if (this.installedObject != null) {
            Debug.LogError("Tile - Trying to place InstalledObject over an existing object.");
            return false;
        }

        this.installedObject = installedObject;
        return true;
    }

    public abstract void ChangeSprite(SpriteRenderer spriteRenderer);

}