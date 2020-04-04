using System;
using UnityEngine;

public class Character {

    public float x => Mathf.Lerp(currentTile.x, destinationTile.x, movementProgressPercentage);
    public float y => Mathf.Lerp(currentTile.y, destinationTile.y, movementProgressPercentage);

    Tile currentTile;
    Tile destinationTile;
    float movementProgressPercentage;
    float speed;

    Job job;

    Action<Character> cbMoved;

    public Character(Tile spawnTile, float speed = 3) {
        this.currentTile = this.destinationTile = spawnTile;
        this.speed = speed;
    }

    public void Update(float deltaTime) {
        
        // Get a new job if we don't have one yet...
        if (job == null) {
            // Try and get a job from the JobQueue (if there is one available).
            job = currentTile.world.jobQueue.Dequeue();

            if (job != null) {
                // We now have a new job.
                destinationTile = job.tile;
                job.RegisterJobCancelCallback(OnJobEnded);
                job.RegisterJobCompleteCallback(OnJobEnded);
            }
        }
        
        // Check if we have already arrived at the Tile
        if (currentTile == destinationTile) {
            if (job != null) {
                job.Work(deltaTime);
            }
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
            currentTile = destinationTile;
        }

        cbMoved?.Invoke(this);
    }

    public void SetDestination(Tile destinationTile) {
        if (currentTile.IsNeighbour(destinationTile, true) == false) {
            Debug.Log("SetDestination - Destination Tile is not the character's neighbour");
        }

        this.destinationTile = destinationTile;
    }
    
    void OnJobEnded(Job job) {
        if (this.job != job) {
            Debug.Log("OnJobEnded - Character is being told about a job other than it's own. Something was probably not Unregistered properly!");
        }

        this.job = null;
    }

    public void RegisterMovedCallback(Action<Character> callback) {
        cbMoved += callback;
    }

    public void UnregisterMovedCallback(Action<Character> callback) {
        cbMoved -= callback;
    }

}
