using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerseComponent : ObserverBehaviour<Verse> {
    EntityComponent<ConcreteEntity> selectorComponent;

    EntityComponent<ConcreteEntity> SpawnEntity(ConcreteEntity entity) {
        GameObject o = new GameObject();
        o.name = entity.Name;

        EntityComponent<ConcreteEntity> component;
        if (entity is Projectile) {
            component = o.AddComponent<ProjectileComponent>();
            entity.register(component);
        } else {
            component = o.AddComponent<EntityComponent>();
            entity.register(component);
        }
        return component;
    }

    override public void HandleEvent(string signal, object[] args) {
        switch (signal) {
            case "SpawnEntity":
                SpawnEntity(args[0] as ConcreteEntity);
                break;
            case "OnActiveShipChanged":
                ShipComponent sc = GetComponent<ShipController>().GetShipComponent(Emitter.ActiveShip());
                selectorComponent.transform.SetParent(sc.transform);
                selectorComponent.transform.localEulerAngles = new Vector3(0, 0, 0);
                GameObject.Find("Main Camera").transform.SetParent(sc.transform);
                GameObject.Find("Main Camera").transform.localEulerAngles = new Vector3(0,0,0);
                GameObject.Find("Main Camera").GetComponent<CameraController>().Center();
                GameObject.Find("Overlay").transform.SetParent(sc.transform);
                GameObject.Find("Overlay").transform.localEulerAngles = new Vector3(0, 0, 0);
                GameObject.Find("Overlay").transform.localPosition = new Vector3(0, 0, 0);
                break;
            case "SpawnSelectionEntity":
                EntityComponent<ConcreteEntity> c = SpawnEntity(args[0] as ConcreteEntity);
                selectorComponent = c;
                break;
        }
    }

    override public void HandleEvent(string signals) {
        base.HandleEvent(signals);
    }
}
