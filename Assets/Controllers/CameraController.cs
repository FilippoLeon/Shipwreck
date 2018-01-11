using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class CameraController : MonoBehaviour {

    public GameObject background;

    int ppu = 32;
    int ppuScale = 1;

    //public float maxZoom = 0.1f;
    //public float minZoom = 30f;
    public float cameraZ = -10f;
    //public float zoomSpeed = 0.1f;
    public float panSpeed = 5f;
    public float slideSpeed = 0.5f;

    Vector3 grabPosition;
    Vector3 initialPos;

    void Update() {
        GetComponent<Camera>().orthographicSize = Screen.height / (ppu * ppuScale) / 2.0f;
    
        float speedX = Input.GetAxis("Horizontal");
        float speedY = Input.GetAxis("Vertical");

        Vector3 pos = Camera.main.transform.position;
        //float zoom = Input.GetAxis("Zoom");
        //zoom *= 0.01f * zoom * zoom;

        pos += new Vector3(speedX, speedY, 0) * Camera.main.orthographicSize * slideSpeed;
        //Camera.main.orthographicSize += zoom * zoomSpeed;

        //// DRAG
        ////if(Input.GetButtonDown("Pan"))
        ////{
        ////    grabPosition = Input.mousePosition;
        ////} else if (Input.GetButton("Pan"))
        ////{
        ////    Camera.main.transform.position += Camera.main.ScreenToViewportPoint(grabPosition - Input.mousePosition);
        ////}

        //// GRAB
        if (Input.GetButton("Pan")) {
            pos += panSpeed * Camera.main.orthographicSize * Camera.main.ScreenToViewportPoint(grabPosition - Input.mousePosition);
        }

        //// GRAB2
        //if (Input.GetButtonDown("Pan")) {
        //    grabPosition = Input.mousePosition;
        //    initialPos = pos;
        //} else if (Input.GetButton("Pan")) {
        //    initialPos += new Vector3(speedX, speedY, 0) * Camera.main.orthographicSize * slideSpeed;
        //    pos = initialPos + panSpeed * Camera.main.orthographicSize * Camera.main.ScreenToViewportPoint(grabPosition - Input.mousePosition);
        //}

        pos = RestoreCameraWithinBounds(pos);

        Camera.main.transform.position = pos;
        background.transform.position = new Vector3(pos.x * 0.1f, pos.y * 0.1f, 20);
        //// GRAB
        grabPosition = Input.mousePosition;

    }

    Vector3 RestoreCameraWithinBounds(Vector3 cameraPos) {
        cameraPos.x = Mathf.Clamp(cameraPos.x, -10, 10);
        cameraPos.y = Mathf.Clamp(cameraPos.y, -10, 10);
        return cameraPos;

        //Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, maxZoom, minZoom);
    }

    public void Center() {
        Camera.main.transform.position = new Vector3(0f, 0f, cameraZ);
    }
}