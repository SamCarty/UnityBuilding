
using System.Collections.Generic;
using UnityEngine;

public class Wall : InstalledObject {

    public Wall() {
        objectSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/InstalledObjects/");
        foreach (Sprite sprite in sprites) {
            objectSprites.Add(sprite.name, sprite);
        }
        CreatePrototype(this, 0, true, 1, 1);
    }
    
    protected override InstalledObject GenerateNewInstalledObject() {
        return new Wall();
    }

    public override void ChangeSprite(SpriteRenderer spriteRenderer, World world) {
        spriteRenderer.sprite = GetSpriteForInstalledObject(world, this);;
        spriteRenderer.sortingLayerName = "InstalledObject";
    }
}
