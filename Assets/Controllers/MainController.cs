using LUA;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainController : MonoBehaviour {

    ScriptLoader scriptLoader;
    public Verse Verse { set; get; }

    ShipController shipController;

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
        OverlayComponent overlayComponent = overlay.AddComponent<OverlayComponent>();
        Verse.register(overlayComponent);

        Verse.Start();
    }

    // Update is called once per frame
    void Update () {
        Verse.Update();
	}
}
