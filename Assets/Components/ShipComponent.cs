using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipComponent : ObserverBehaviour<Ship> {
    void AddPart(Part part) {
        GameObject o = new GameObject();
        PartComponent partComponent = o.AddComponent<PartComponent>();

        partComponent.transform.SetParent(transform);
        o.name = "Part";

        part.register(partComponent);
    }

    override public void HandleEvent(string signal, object[] args) {
        switch(signal) {
            case "AddPart":
                AddPart(args[0] as Part);
                break;
        }
    }

    override public void HandleEvent(string signals) {

    }
}
