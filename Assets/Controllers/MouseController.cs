using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class MouseController : MonoBehaviour {
    public GameObject cursor;

    Camera camera;
    Vector3 lastFramePos;
    Vector3 dragStartPos;

    // Start is called before the first frame update
    void Start() {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        Vector3 currentFramePos = camera.ScreenToWorldPoint(Input.mousePosition);
        currentFramePos.z = 0;

        // Update the cursor position
        Tile tileUnderCursor = GetTileAtCoords(currentFramePos);
        if (tileUnderCursor != null) {
            cursor.SetActive(true);
            Vector3 cursorPos = new Vector3(tileUnderCursor.X, tileUnderCursor.Y);
            cursor.transform.position = cursorPos;
        }
        else {
            cursor.SetActive(false);
        }

        // Start left button drag
        if (Input.GetMouseButtonDown(0)) {
            dragStartPos = currentFramePos;
        }

        // End left button drag
        if (Input.GetMouseButtonUp(0)) {
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

            // Loop through all the tiles in the selection and change their type.
            for (int x = startX; x <= endX; x++) {
                for (int y = startY; y <= endY; y++) {
                    Tile tile = WorldController.Instance.World.GetTileAt(x, y);
                    if (tile != null) tile.Type = Tile.TileType.Floor;
                }
            }
        }

        // Drag camera around the scene.
        if (Input.GetMouseButton(1)) {
            Vector3 diff = lastFramePos - currentFramePos;
            camera.transform.Translate(diff);
        }

        lastFramePos = camera.ScreenToWorldPoint(Input.mousePosition);
        lastFramePos.z = 0;
    }

    Tile GetTileAtCoords(Vector3 coords) {
        int x = Mathf.FloorToInt(coords.x);
        int y = Mathf.FloorToInt(coords.y);

        return WorldController.Instance.World.GetTileAt(x, y);
    }
}