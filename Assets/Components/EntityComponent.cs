using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityComponent<T> : SpritedObserverBehaviour<T> where T : ConcreteEntity<T> {

    void Start() {

    }

    void SpawnAt(Vector2 coordinate) {
        transform.localPosition = coordinate;
        name = Emitter.Name;

        base.CreateGraphics();
    }

    override public void HandleEvent(string signal, object[] args) {
        switch(signal) {
            case "SpawnAt":
                SpawnAt((Vector2) args[0]);
                break;
            case "SetActive":
                gameObject.SetActive((bool) args[0]);
                break;
            case "SetPosition":
                transform.localPosition = ((Vector2)args[0]);
                break;
            case "SetAngle":
                transform.localEulerAngles = new Vector3(0,0,(float) args[0]);
                break;
            //default:
            //    base.HandleEvent(signal, args);
            //    break;
        }
    }

    override public void HandleEvent(string signals) {
        switch (signals) {
            case "OnFacingChanged":
                transform.localEulerAngles = new Vector3(0, 0, 90 * (int) Emitter.Facing);
                break;
            default:
                base.HandleEvent(signals);
                break;
        }
    }

    protected void Update() {
        base.UpdateGraphics();
    }
}

public class EntityComponent : EntityComponent<ConcreteEntity> { }