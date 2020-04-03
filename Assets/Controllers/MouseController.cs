using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {
    
    // Camera
    private Camera camera;
    private Vector3 lastFramePos;
    private Vector3 currentFramePos;

    // Dragging to place Tiles/Objects
    [SerializeField] private GameObject cursorPrefab;
    private Vector3 dragStartPos;
    private List<GameObject> dragPreviewObjects;

    BuildModeController buildModeController;

    // Start is called before the first frame update
    void Start() {
        camera = Camera.main;
        dragPreviewObjects = new List<GameObject>();
        buildModeController = FindObjectOfType<BuildModeController>();
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
                        if (tile != null) { buildModeController.Build(tile); }
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
}