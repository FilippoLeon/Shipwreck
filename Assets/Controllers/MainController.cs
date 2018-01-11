using LUA;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainController : MonoBehaviour {

    ScriptLoader scriptLoader;
    public Verse Verse { set; get; }

    // Use this for initialization
    void Start () {
        string pathXml = Path.Combine(Application.streamingAssetsPath, "Data/Prototypes/Parts.xml");

        scriptLoader = new ScriptLoader();

        Verse = new Verse();

        Verse.registry.partRegistry.ReadPrototypes(pathXml);

        gameObject.AddComponent<SpriteController>();
        gameObject.AddComponent<GUIController>();
        gameObject.AddComponent<ShipController>();
        gameObject.AddComponent<InputController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
