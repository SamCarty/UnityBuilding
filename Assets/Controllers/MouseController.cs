using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = System.Diagnostics.Debug;

public class MouseController : MonoBehaviour {
    public GameObject cursorPrefab;

    Tile.TileType buildModeTile = Tile.TileType.Floor;

    Camera camera;
    Vector3 lastFramePos;
    Vector3 currentFramePos;
    
    Vector3 dragStartPos;
    List<GameObject> dragPreviewObjects;

    // Start is called before the first frame update
    void Start() {
        camera = Camera.main;
        dragPreviewObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update() {
        currentFramePos = camera.ScreenToWorldPoint(Input.mousePosition);
        currentFramePos.z = 0;

        //UpdateCursor();
        UpdateTileDragging();
        UpdateCameraMovement();
        
        lastFramePos = camera.ScreenToWorldPoint(Input.mousePosition);
        lastFramePos.z = 0;
    }

/*    void UpdateCursor() {
        // Update the cursor position
        Tile tileUnderCursor = WorldController.Instance.GetTileAtCoords(currentFramePos);
        if (tileUnderCursor != null) {
            cursor.SetActive(true);
            Vector3 cursorPos = new Vector3(tileUnderCursor.X, tileUnderCursor.Y);
            cursor.transform.position = cursorPos;
        }
        else {
            cursor.SetActive(false);
        }
    }*/

    void UpdateTileDragging() {
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
                    if (x >= 0 && y >= 0) { // Checks tile is in range.
                        Tile tile = WorldController.Instance.World.GetTileAt(x, y);
                        if (tile != null) {
                            GameObject gameObject =
                                SimplePool.Spawn(cursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
                            dragPreviewObjects.Add(gameObject);
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
                    if (x >= 0 && y >= 0) { // Checks tile is in range.
                        Tile tile = WorldController.Instance.World.GetTileAt(x, y);
                        if (tile != null) tile.Type = buildModeTile;
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
        buildModeTile = Tile.TileType.Floor;
    }

    public void SetModeBulldoze() {
        buildModeTile = Tile.TileType.Empty;
    }
}
