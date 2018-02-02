using LUA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainController : MonoBehaviour {

    ScriptLoader scriptLoader = null;
    public Verse Verse { set; get; }

    ShipController shipController;
    OverlayComponent overlayComponent;

    public GameObject starmapVertex;

    public GameObject planetTemplate;
    
    public Material starmapEdgeMaterial;

    public GameObject galaxy;
    public GameObject solarMap;

    internal float tacticalStartTime = 0;
    internal float nonTacticalStartTime = 0;
    float hitTime;
    GameObject oldHitObject;
    public ViewMode viewMode { get; private set; }

    public IView CurrentView {
        get; private set;
    }

    public enum ViewMode {
        Ship, Map, TacticalMap, SolarMap
    }

    public void SetMode(ViewMode mode, IView view = null) {
        if (mode == ViewMode.TacticalMap || viewMode != ViewMode.TacticalMap ) {
            tacticalStartTime = Time.time;
        } else if (mode != ViewMode.TacticalMap || viewMode == ViewMode.TacticalMap) {
            nonTacticalStartTime = Time.time;
        }
        viewMode = mode;
        switch (mode) {
            case ViewMode.Ship:
                CurrentView = Verse;
                galaxy.SetActive(false);
                break;
            case ViewMode.Map:
                CurrentView = Verse.Galaxy;
                galaxy.SetActive(true);
                break;
            case ViewMode.SolarMap:
                CurrentView = view;
                galaxy.SetActive(false);
                break;
            case ViewMode.TacticalMap:
                CurrentView = null;
                break;
        }
        if(mode != ViewMode.SolarMap && solarMap != null) {
             Destroy(solarMap);
        }
    }

    private CameraController cameraController;

        // Use this for initialization
    void Start () {
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();


        scriptLoader = new ScriptLoader();

        Verse = new Verse();

        SpriteController spriteController = gameObject.AddComponent<SpriteController>();
        spriteController.Load();

        string pathXml = Path.Combine(Application.streamingAssetsPath, "Data/Prototypes/Parts.xml");
        Verse.registry.partRegistry.ReadPrototypes(pathXml);
        pathXml = Path.Combine(Application.streamingAssetsPath, "Data/Prototypes/Entities.xml");
        Verse.registry.entityRegistry.ReadPrototypes(pathXml);

        gameObject.AddComponent<GUIController>();
        shipController = gameObject.AddComponent<ShipController>();
        gameObject.AddComponent<InputController>();

        VerseComponent verseComponent = gameObject.AddComponent<VerseComponent>();
        Verse.register(verseComponent);

        GameObject overlay = new GameObject("Overlay");
        overlay.transform.SetParent(cameraController.transform);
        overlayComponent = overlay.AddComponent<OverlayComponent>();
        Verse.register(overlayComponent);

        galaxy = new GameObject("StarMap");
        GalaxyComponent galaxyComponent = galaxy.AddComponent<GalaxyComponent>();
        galaxyComponent.Camera = GameObject.Find("MapCamera").GetComponent<Camera>();
        galaxyComponent.vert = starmapVertex;
        galaxyComponent.starPathMaterial = starmapEdgeMaterial;

        Verse.Galaxy.register(galaxyComponent);

        Verse.Create();

        Verse.SetMap("Health");

        SetMode(ViewMode.Ship);
    }
    
    // Update is called once per frame
    void Update () {
        Verse.Update();

        if( Input.GetKeyDown(KeyCode.Tab)) {
            Verse.NextShip();
        }

        if( Input.GetButtonDown("map")) {
            switch (viewMode) {
                case ViewMode.Ship:
                case ViewMode.TacticalMap:
                    SetMode(ViewMode.Map);
                    break;
                case ViewMode.Map:
                case ViewMode.SolarMap:
                    SetMode(ViewMode.Ship);
                    break;
            }
        } else if ( Input.GetButtonDown("TacticalMap") ) {
            switch (viewMode) {
                case ViewMode.Ship:
                case ViewMode.Map:
                case ViewMode.SolarMap:
                    SetMode(ViewMode.TacticalMap);
                    break;
                case ViewMode.TacticalMap:
                    SetMode(ViewMode.Ship);
                    break;
            }
        }
        if (viewMode == ViewMode.Map || viewMode == ViewMode.SolarMap ) {
            if( Input.GetMouseButtonDown(0) ) {
                Camera camera = galaxy.GetComponent<GalaxyComponent>().Camera;
                Vector2 ray = camera.ScreenToWorldPoint( Input.mousePosition );

                RaycastHit2D hitInfo = Physics2D.Raycast(ray, ray, 0.0f);

                if (viewMode == ViewMode.Map && hitInfo.transform != null && hitInfo.transform.GetComponent<SolarSystemComponent>() != null) {
                    if (hitInfo.transform.gameObject == oldHitObject && (Time.time - hitTime) < 0.5f) {
                        SolarSystemComponent ssc = hitInfo.transform.GetComponent<SolarSystemComponent>();
                        solarMap = ssc.DisplaySystem();
                        SetMode(ViewMode.SolarMap, ssc.Emitter);

                        Debug.Log(ssc.Emitter.Name);
                    }

                    oldHitObject = hitInfo.transform.gameObject;
                    hitTime = Time.time;
                    //GUIController.childs["star_menu"].Show();
                } else if (viewMode == ViewMode.SolarMap && hitInfo.transform != null && hitInfo.transform.GetComponentInParent<PlanetComponent>() != null) {
                    PlanetComponent pc = hitInfo.transform.GetComponentInParent<PlanetComponent>();
                    GUIController.childs["planet_menu"].Show();
                    GUIController.childs["planet_menu"].GameObject.transform.position = camera.WorldToScreenPoint(pc.transform.position);
                    GUIController.childs["planet_menu"].SetParameters(new object[] { pc.Emitter });
                } else if (hitInfo.transform == null || hitInfo.transform.GetComponent<WidgetComponent>() == null) {
                    if (GUIController.HoverObject == null) {
                        //GUIController.childs["star_menu"].Hide();
                        GUIController.childs["planet_menu"].Hide();
                    }
                }
            }
            
        }

        Vector3 mousePos = cameraController.mainCamera.ScreenToViewportPoint(Input.mousePosition);
        if (cameraController.tacticalMapCamera.rect.Contains(mousePos)) {
            //Debug.Log("Mouse is on tacmap.");
            if ( Input.GetButtonDown("Fire1") ) { 
                shipController.Ship.AddWaypoint(
                     cameraController.tacticalMapCamera.ScreenToWorldPoint(Input.mousePosition)
                    );
                //Debug.Log(cameraController.tacticalMapCamera.ScreenToWorldPoint(Input.mousePosition));
            }
        }
    }

    public GameObject GetPlanet(Planet p = null) {
        GameObject ret = Instantiate(planetTemplate);
        if (p != null) {
            ret.AddComponent<PlanetComponent>().Emitter = p;
        }

        return ret;
    }
}