using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour {
    World world => WorldController.instance.world;

    // Map of all InstalledObjects logic elements and their corresponding GameObject in the editor.
    Dictionary<Character, GameObject> characterGameObjectMap;
    Dictionary<string, Sprite> characterObjectSprites;

    // Start is called before the first frame update
    void Start() {
        LoadSpritesFromFile();

        // Initialize map that tracks the GameObject that is representing the InstalledObject data.
        characterGameObjectMap = new Dictionary<Character, GameObject>();

        // Register our callback events
        world.RegisterCharacterPlaced(OnCharacterPlaced);
        
        // TODO: For debug only
        Character character = world.CreateCharacter(world.GetTileAt(world.width / 2, world.height / 2));
        character.SetDestination(world.GetTileAt(world.width / 2 + 5, world.height / 2));
        character.RegisterMovedCallback(OnCharacterMoved);
    }

    void LoadSpritesFromFile() {
        characterObjectSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Characters/");
        foreach (Sprite sprite in sprites) {
            characterObjectSprites.Add(sprite.name, sprite);
        }
    }

    public void OnCharacterPlaced(Character character) {
        // Create the visual representation (GameObject) of the Character.
        GameObject gameObject = new GameObject();
        gameObject.name = "Character_";
        gameObject.transform.position = new Vector3(character.x, character.y);
        gameObject.transform.SetParent(transform, true);

        // Add the InstalledObject data and GameObject to the map.
        characterGameObjectMap.Add(character, gameObject);

        // Add a SpriteRenderer to the InstalledObject.
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = characterObjectSprites["survivor1_stand"];
        spriteRenderer.sortingLayerName = "Character";

        // Register the Character changed callback.
        //character.RegisterCharacterChanged(OnCharacterChanged);
    }

    void OnCharacterMoved(Character character) {
        // Make sure the InstalledObject graphics are updated when an adjacent thing is added.
        if (characterGameObjectMap.TryGetValue(character, out var obj)) {
            obj.transform.position = new Vector3(character.x, character.y, 0);
        }
        else {
            Debug.LogError("OnCharacterMoved - Character is not present in the " +
                           "characterGameObjectMap.");
        }
    }
}