using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerseComponent : ObserverBehaviour<Verse> {
    void SpawnEntity(ConcreteEntity entity) {
        GameObject o = new GameObject();
        EntityComponent entityComponent = o.AddComponent<EntityComponent>();

        entityComponent.transform.SetParent(transform);
        o.name = "Entity";

        entity.register(entityComponent);
    }

    override public void HandleEvent(string signal, object[] args) {
        switch (signal) {
            case "SpawnEntity":
                SpawnEntity(args[0] as ConcreteEntity);
                break;
        }
    }

    override public void HandleEvent(string signals) {

    }
}
