﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {
    // Build mode on and off
    private bool buildModeIsObjects = false;
    private InstalledObjectType buildModeInstalledObjectType;
    private TileType buildModeTile = TileType.Floor;

    // Camera
    private Camera camera;
    private Vector3 lastFramePos;
    private Vector3 currentFramePos;

    // Dragging to place Tiles/Objects
    [SerializeField] private GameObject cursorPrefab;
    private Vector3 dragStartPos;
    private List<GameObject> dragPreviewObjects;

    // Start is called before the first frame update
    void Start() {
        camera = Camera.main;
        dragPreviewObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update() {
        currentFramePos = camera.ScreenToWorldPoint(Input.mousePosition);
        currentFramePos.z = 0;

        UpdateTileDragging();
        UpdateCameraMovement();

        lastFramePos = camera.ScreenToWorldPoint(Input.mousePosition);
        lastFramePos.z = 0;
    }

    void UpdateTileDragging() {
        // If mouse is over a user element, do nothing!
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // Start left button drag
        if (Input.GetMouseButtonDown(0)) {
            dragStartPos = currentFramePos;
        }

        int startX = Mathf.FloorToInt(dragStartPos.x);
        int endX = Mathf.FloorToInt(currentFramePos.x);
        // Flip coords if they are wrong way round to avoid negatives in loop.
        if (startX > endX) {
            int temp = endX;
            endX = startX;
            startX = temp;
        }

        int startY = Mathf.FloorToInt(dragStartPos.y);
        int endY = Mathf.FloorToInt(currentFramePos.y);
        // Flip coords if they are wrong way round to avoid negatives in loop.
        if (startY > endY) {
            int temp = endY;
            endY = startY;
            startY = temp;
        }

        dragPreviewObjects.ForEach(obj => SimplePool.Despawn(obj));
        dragPreviewObjects.Clear();


        // While mouse button is held down, display preview of the drag area.
        if (Input.GetMouseButton(0)) {
            for (int x = startX; x <= endX; x++) {
                for (int y = startY; y <= endY; y++) {
                    if (x >= 0 && y >= 0) {
                        // Checks tile is in range.
                        Tile tile = WorldController.instance.world.GetTileAt(x, y);
                        if (tile != null) {
                            GameObject cursorOverlayGameObject =
                                SimplePool.Spawn(cursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
                            dragPreviewObjects.Add(cursorOverlayGameObject);
                        }
                    }
                }
            }
        }

        // End left button drag
        if (Input.GetMouseButtonUp(0)) {
            // Loop through all the tiles in the selection and change their type.
            for (int x = startX; x <= endX; x++) {
                for (int y = startY; y <= endY; y++) {
                    // Checks tile is in range.
                    if (x >= 0 && y >= 0) {
                        Tile tile = WorldController.instance.world.GetTileAt(x, y);

                        if (buildModeIsObjects) {
                            // Create the InstalledObject and assign it to the designated Tile.

                            // Check legality of placing InstalledObject here.
                            if (InstalledObject.CheckPlacementValidity(buildModeInstalledObjectType, tile)) {
                                // Create the InstalledObject as a new pending Job.
                                Job job = new Job(tile,
                                    (j) => {
                                        WorldController.instance.world.PlaceInstalledObject(
                                            buildModeInstalledObjectType, tile);
                                    });

                                // TODO: probably should move to somewhere else, too easy to forget to do this!!
                                tile.pendingInstalledObjectJob = job;
                                
                                job.RegisterJobCancelCallback(j => { tile.pendingInstalledObjectJob = null; });
                                
                                // Add the job to the World's job queue.
                                WorldController.instance.world.jobs.Enqueue(job);
                                Debug.Log("Job queue size: " + WorldController.instance.world.jobs.Count);
                            }
                        }

                        else {
                            // We are in Tile changing mode, not object mode.
                            if (tile != null) tile.tileType = buildModeTile;
                        }
                    }
                }
            }
        }
    }

    void UpdateCameraMovement() {
// Drag camera around the scene.
        if (Input.GetMouseButton(1)) {
            Vector3 diff = lastFramePos - currentFramePos;
            camera.transform.Translate(diff);
        }

        camera.orthographicSize -= camera.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 3f, 30f);
    }

    public void SetModeBuildFoundation() {
        buildModeTile = TileType.Floor;
        buildModeIsObjects = false;
    }

    public void SetModeBuildInstalledObject(string type) {
// Wall is not a TileType, it is an InstalledObject!
        buildModeIsObjects = true;

// Try and get the Enum ObjectType from the passed in string (workaround as Unity does not allow Enums to be
// passed into script methods from the editor.
        try {
            InstalledObjectType objectType =
                (InstalledObjectType) System.Enum.Parse(typeof(InstalledObjectType), type);
            buildModeInstalledObjectType = objectType;
        }
        catch (System.Exception) {
            Debug.LogError("MouseController - Parse cannot convert the ObjectType string to an Enum, " +
                           "check the spelling.");
        }
    }

    public void SetModeBulldoze() {
        buildModeTile = TileType.Ground;
        buildModeIsObjects = false;
    }
}