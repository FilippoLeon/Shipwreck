using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

    Verse verse;

    void Start() {
        verse = GetComponent<MainController>().Verse;
    }

    void Update() {
        if (verse.verseMode == Verse.VerseMode.Build) {
            if (Input.GetButtonDown("Fire1")) {
                Coordinate coord = new Coordinate(
                    Camera.main.ScreenToWorldPoint(Input.mousePosition)
                    );
                GetComponent<ShipController>().Ship.AddPart(verse.registry.partRegistry.Get(verse.modeArgs[0] as string), coord);
            }
        }
    }
}
