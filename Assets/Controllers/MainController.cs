using LUA;
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

    public Material starmapEdgeMaterial;

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

        GameObject galaxy = new GameObject("StarMap");
        GalaxyComponent galaxyComponent = galaxy.AddComponent<GalaxyComponent>();
        galaxyComponent.vert = starmapVertex;
        galaxyComponent.starPathMaterial = starmapEdgeMaterial;

        Verse.Galaxy.register(galaxyComponent);

        Verse.Create();

        Verse.SetMap("Health");
    }

    // Update is called once per frame
    void Update () {
        Verse.Update();
	}
}
