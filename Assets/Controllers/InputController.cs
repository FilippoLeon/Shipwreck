using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

    Verse verse;

    void Start() {
        verse = GetComponent<MainController>().Verse;
    }

    void Update() {
        ShipComponent activeShipComponent = GetComponent<ShipController>().GetShipComponent(verse.ActiveShip());
        Vector3 localPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - activeShipComponent.transform.position;
        localPos.z = 0;
        Vector3 rotatedPos = Quaternion.AngleAxis(-activeShipComponent.transform.localEulerAngles.z, Vector3.forward) * localPos;
        Coordinate coord = new Coordinate(
             rotatedPos
            );
        if (verse.verseMode == Verse.VerseMode.Build) {
            if (Input.GetButtonDown("Fire1")) {
                activeShipComponent.Emitter.AddPart(verse.registry.partRegistry.Get(verse.modeArgs[0] as string), coord);
            }
        }
        if (Input.GetButtonDown("Fire3")) {
            verse.Select(coord);
        }
    }
}
