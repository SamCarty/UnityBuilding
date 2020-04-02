using System.Collections.Generic;
using UnityEngine;

public class Ground: Tile {
    public Ground(World world, int x, int y) {
        Dictionary<string, Sprite> groundSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Tiles/");
        foreach (Sprite sprite in sprites) {
            groundSprites.Add(sprite.name, sprite);
        }
        
        Debug.Log(groundSprites);

        CreateTile(world, x, y, groundSprites["Grass_A"]);
    }

    public Ground(Tile parent) {
        Dictionary<string, Sprite> groundSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Tiles/");
        foreach (Sprite sprite in sprites) {
            groundSprites.Add(sprite.name, sprite);
        }
        
        Debug.Log(groundSprites);
        
        CreateTile(parent.world, parent.x, parent.y, groundSprites["Grass_A"]);
    }
    
    public override void ChangeSprite(SpriteRenderer spriteRenderer) {
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingLayerName = "Ground";
    }
    
}