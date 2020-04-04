﻿using UnityEngine;

public class WorldController : MonoBehaviour {
    
    // By keeping an instance of ourselves, we can ensure there is only 1 WorldController present.
    public static WorldController instance { get; private set; }
    
    // Our world that we are controlling right now.
    public World world { get; private set; }

    // Start is called before the first frame update.
    void Start() {
        if (instance != null) {
            Debug.LogError("There should never be more than one WorldController!");
        }
        else {
            instance = this;
        }

        // Create world with empty tiles.
        world = new World();

        // Position the camera in the centre of the world.
        if (Camera.main != null) {
            var cameraTransform = Camera.main.transform;
            cameraTransform.position = new Vector3(world.width / 2, world.height / 2,
                cameraTransform.position.z);
        }
    }
    
    void Update() {
        // TODO add pause, speed controls etc.
        
        // TODO this is for testing the job system works, to be removed after!
        foreach (Job job in world.jobQueue.jobs) {
            job.Work(Time.deltaTime);
        }
        
        world.Simulate(Time.deltaTime);
    }
    
    public Tile GetTileAtCoords(Vector3 coords) {
        int x = Mathf.FloorToInt(coords.x);
        int y = Mathf.FloorToInt(coords.y);

        return world.GetTileAt(x, y);
    }
}