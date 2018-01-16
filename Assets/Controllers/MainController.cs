using LUA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainController : MonoBehaviour {

    ScriptLoader scriptLoader;
    public Verse Verse { set; get; }

    ShipController shipController;
    OverlayComponent overlayComponent;

    public GameObject starmapVertex;

    public GameObject planetTemplate;
    
    public Material starmapEdgeMaterial;

    public GameObject galaxy;

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
    }

        // Use this for initialization
       void Start () {
        string pathXml = Path.Combine(Application.streamingAssetsPath, "Data/Prototypes/Parts.xml");

        scriptLoader = new ScriptLoader();

        Verse = new Verse();

        SpriteController spriteController = gameObject.AddComponent<SpriteController>();
        spriteController.Load();

        Verse.registry.partRegistry.ReadPrototypes(pathXml);

        gameObject.AddComponent<GUIController>();
        ShipController shipController = gameObject.AddComponent<ShipController>();
        gameObject.AddComponent<InputController>();

        VerseComponent verseComponent = gameObject.AddComponent<VerseComponent>();
        Verse.register(verseComponent);

        GameObject overlay = new GameObject("Overlay");
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
        if (viewMode == ViewMode.Map) {
            if( Input.GetMouseButtonDown(0) ) {
                UnityEngine.Ray ray = galaxy.GetComponent<GalaxyComponent>().Camera.ScreenPointToRay( Input.mousePosition );

                RaycastHit hitInfo;
                bool hit = Physics.Raycast(ray, out hitInfo);

                if( hit && hitInfo.transform.GetComponent<SolarSystemComponent>() != null ) {
                    if ( hitInfo.transform.gameObject == oldHitObject && (Time.time - hitTime) < 0.5f) {
                        SolarSystemComponent ssc = hitInfo.transform.GetComponent<SolarSystemComponent>();
                        ssc.DisplaySystem();
                        SetMode(ViewMode.SolarMap, ssc.Emitter);

                        Debug.Log( ssc.Emitter.Name );
                    }
                   
                    oldHitObject = hitInfo.transform.gameObject;
                    hitTime = Time.time;
                }
            }
            
        }
    }

    public GameObject GetPlanet() {
        GameObject ret = Instantiate(planetTemplate);

        return ret;
    }
}