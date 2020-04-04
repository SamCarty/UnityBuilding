using UnityEngine;

public class Character {

    public float x => Mathf.Lerp(currentTile.x, destinationTile.x, movementProgressPercentage);
    public float y => Mathf.Lerp(currentTile.y, destinationTile.y, movementProgressPercentage);

    Tile currentTile;
    Tile destinationTile;
    float movementProgressPercentage;
    float speed;

    public Character(Tile spawnTile, float speed = 5) {
        this.currentTile = this.destinationTile = spawnTile;
        this.speed = speed;
    }

    public void Move(float deltaTime) {
        Debug.Log("Character update method called...");
        
        // Check if we have already arrived at the Tile
        if (currentTile == destinationTile) {
            return;
        }
        
        // Get the distance between the current Tile and the destination Tile using Pythagoras' Theorem!
        float distance = Mathf.Sqrt(Mathf.Pow(currentTile.x - destinationTile.x, 2) + Mathf.Pow(currentTile.y - destinationTile.y, 2));
        
        // Work out how far we should travel this frame.
        float distanceThisFrame = speed * deltaTime;
        
        // How far are we now between our current Tile and the destination Tile?
        float movementProgressThisFrame = distanceThisFrame / distance;
        
        // Add to the overall percentage travelled.
        movementProgressPercentage += movementProgressThisFrame;
        
        // Adjust for overshooting the destination.
        if (movementProgressPercentage >= 1) {
            // Destination reached!
            movementProgressPercentage = 0;
        }

        currentTile = WorldController.instance.world.GetTileAt((int) x, (int) y);
    }

    public void SetDestination(Tile destinationTile) {
        if (currentTile.IsNeighbour(destinationTile, true) == false) {
            Debug.Log("SetDestination - Destination Tile is not the character's neighbour");
        }

        this.destinationTile = destinationTile;

    }
    
    
    
}
