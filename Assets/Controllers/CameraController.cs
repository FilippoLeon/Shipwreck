﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class CameraController : MonoBehaviour {

    public GameObject background = null;

    int ppu = 32;
    int ppuScale = 1;

    public float maxZoom = 6f;
    public float minZoom = 50f;
    public float cameraZ = -10f;
    public float zoomSpeed = 0.1f;
    public float panSpeed = 5f;
    public float slideSpeed = 0.5f;

    [Range(-1, 1)]
    public float parallax = -0.03f;

    Vector3 grabPosition;
    Vector3 initialPos;

    MainController main;
    internal Camera mainCamera;
    internal Camera tacticalMapCamera;

    float size = 0;

    void Start() {
        main = FindObjectOfType<MainController>();
        mainCamera = GetComponent<Camera>();
        tacticalMapCamera = GameObject.Find("TacticalMapCamera").GetComponent<Camera>();

        mainCamera.orthographicSize = Screen.height / (ppu * ppuScale) / 2.0f;
    }

    enum ZoomMode {
        Free, PixelPerfect,
        None,
    }

    ZoomMode zoomMode;

    void Update() {
        Camera cameraToProcess = null;
        if (main.viewMode == MainController.ViewMode.Map || main.viewMode == MainController.ViewMode.SolarMap ) {
            cameraToProcess = main.galaxy.GetComponent<GalaxyComponent>().Camera;
            zoomMode = ZoomMode.Free;
        } else if (main.viewMode == MainController.ViewMode.TacticalMap ) {
            cameraToProcess = tacticalMapCamera;
            //zoomMode = ZoomMode.None;
        } else {
            cameraToProcess = mainCamera;
            zoomMode = ZoomMode.PixelPerfect;
        }

        switch (zoomMode) {
            case ZoomMode.Free:
                float zoom = Input.GetAxis("Zoom");
                cameraToProcess.orthographicSize += zoom * zoomSpeed;
                break;
            case ZoomMode.PixelPerfect:
                cameraToProcess.orthographicSize = Screen.height / (ppu * ppuScale) / 2.0f;
                break;
            case ZoomMode.None:
                return;
        }

        float speedX = Input.GetAxis("Horizontal");
        float speedY = Input.GetAxis("Vertical");
        

        Vector3 delta = new Vector3(speedX, speedY, 0) * cameraToProcess.orthographicSize * slideSpeed;

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
            delta += panSpeed * cameraToProcess.orthographicSize * Camera.main.ScreenToViewportPoint(grabPosition - Input.mousePosition);
        }

        //// GRAB2
        //if (Input.GetButtonDown("Pan")) {
        //    grabPosition = Input.mousePosition;
        //    initialPos = pos;
        //} else if (Input.GetButton("Pan")) {
        //    initialPos += new Vector3(speedX, speedY, 0) * Camera.main.orthographicSize * slideSpeed;
        //    pos = initialPos + panSpeed * Camera.main.orthographicSize * Camera.main.ScreenToViewportPoint(grabPosition - Input.mousePosition);
        //}
        

        background.transform.localPosition += delta * parallax;
        cameraToProcess.transform.localPosition = RestoreCameraWithinBounds(cameraToProcess.transform.localPosition + delta);
        // Restore ortho size
        cameraToProcess.orthographicSize = Mathf.Clamp(cameraToProcess.orthographicSize, maxZoom, minZoom);

        //// GRAB
        grabPosition = Input.mousePosition;

        if (main.viewMode == MainController.ViewMode.TacticalMap) {
            size += Time.deltaTime;
        } else {
            size -= Time.deltaTime;
        }
        size = Mathf.Clamp(size, 0, cameraZoomTime);

        float alpha = size / cameraZoomTime;
        //float X = Mathf.Lerp(0, 0, cameraZoomTime - main.startTime);
        float X = 0;
        float Y = Mathf.Lerp(0.7f, 0, alpha );
        float W = Mathf.Lerp(0.2f, 1, alpha );
        float H = Mathf.Lerp(0.2f, 1, alpha );
        tacticalMapCamera.rect = new Rect(X, Y, W, H);
    }

    public float cameraZoomTime = 5.0f;

    Vector3 RestoreCameraWithinBounds(Vector3 cameraPos) {
        if(main.CurrentView == null) {
            return cameraPos;
        }
        Coordinate minCoord = main.CurrentView.GetMin();
        Coordinate maxCoord = main.CurrentView.GetMax();

        cameraPos.x = Mathf.Clamp(cameraPos.x, minCoord.x, maxCoord.x);
        cameraPos.y = Mathf.Clamp(cameraPos.y, minCoord.y, maxCoord.y);
        float aparallax = Mathf.Abs(parallax);
        background.transform.localPosition = new Vector3(
            Mathf.Clamp(background.transform.localPosition.x, -10 * aparallax, 10 * aparallax),
            Mathf.Clamp(background.transform.localPosition.y, -10 * aparallax, 10 * aparallax),
            background.transform.localPosition.z
            );
        return cameraPos;
    }

    public void Center() {
        Camera.main.transform.localPosition = new Vector3(0f, 0f, cameraZ);
    }
}