using UnityEngine;

public class Floor: Tile {
    public Floor(World world, int x, int y) {
        CreateTile(world, x, y, Resources.Load<Sprite>("Images/Tiles/Floor_A"));
    }

    public Floor(Tile parent) {
        CreateTile(parent.world, parent.x, parent.y, Resources.Load<Sprite>("Images/Tiles/Floor_A"));
    }
    
    public override void ChangeSprite(SpriteRenderer spriteRenderer) {
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingLayerName = "Floor";
    }

    public override bool IsBuildonable() {
        return true;
    }
}