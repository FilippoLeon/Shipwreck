using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerseComponent : ObserverBehaviour<Verse> {
    EntityComponent entityComponent;

    void SpawnEntity(ConcreteEntity entity) {
        GameObject o = new GameObject();
        entityComponent = o.AddComponent<EntityComponent>();

        o.name = entity.Name;

        entity.register(entityComponent);
    }

    override public void HandleEvent(string signal, object[] args) {
        switch (signal) {
            case "SpawnEntity":
                SpawnEntity(args[0] as ConcreteEntity);
                break;
            case "OnActiveShipChanged":
                ShipComponent sc = GetComponent<ShipController>().GetShipComponent(Emitter.ActiveShip());
                entityComponent.transform.SetParent(sc.transform);
                entityComponent.transform.localEulerAngles = new Vector3(0, 0, 0);
                GameObject.Find("Main Camera").transform.SetParent(sc.transform);
                GameObject.Find("Main Camera").transform.localEulerAngles = new Vector3(0,0,0);
                GameObject.Find("Main Camera").GetComponent<CameraController>().Center();
                GameObject.Find("Overlay").transform.SetParent(sc.transform);
                GameObject.Find("Overlay").transform.localEulerAngles = new Vector3(0, 0, 0);
                break;
        }
    }

    override public void HandleEvent(string signals) {

    }
}
