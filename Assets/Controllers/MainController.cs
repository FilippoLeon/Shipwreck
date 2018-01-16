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
    }

    float hitTime;
    GameObject oldHitObject;

    // Update is called once per frame
    void Update () {
        Verse.Update();

        if( Input.GetButtonDown("map")) {
            galaxy.SetActive(!galaxy.activeSelf);
        }
        if(galaxy.activeSelf) {
            if( Input.GetMouseButtonDown(0) ) {
                UnityEngine.Ray ray = galaxy.GetComponent<GalaxyComponent>().Camera.ScreenPointToRay( Input.mousePosition );

                RaycastHit hitInfo;
                bool hit = Physics.Raycast(ray, out hitInfo);

                if( hit && hitInfo.transform.GetComponent<SolarSystemComponent>() != null ) {
                    if ( hitInfo.transform.gameObject == oldHitObject && (Time.time - hitTime) < 0.5f) {
                        hitInfo.transform.GetComponent<SolarSystemComponent>().DisplaySystem();
                        galaxy.SetActive(false);

                        Debug.Log( hitInfo.transform.GetComponent<SolarSystemComponent>().Emitter.Name );
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
