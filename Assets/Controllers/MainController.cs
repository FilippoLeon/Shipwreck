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

        gameObject.AddComponent<SpriteController>();

        Verse.registry.partRegistry.ReadPrototypes(pathXml);

        gameObject.AddComponent<GUIController>();
        ShipController shipController = gameObject.AddComponent<ShipController>();
        gameObject.AddComponent<InputController>();
	}
	
	// Update is called once per frame
	void Update () {
        Verse.Update();
	}
}
