using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityComponent<T> : ObserverBehaviour<T> where T : ConcreteEntity<T> {
    SpriteRenderer sr;

    void Start() {

    }

    void SpawnAt(Vector2 coordinate) {
        transform.localPosition = coordinate;

        sr = gameObject.AddComponent<SpriteRenderer>();
        
        SpriteController.spriteLoader.LoadIntoSpriteRenderer(sr, Emitter.spriteInfo, Emitter);
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
        SpriteLoader.SpriteContainer sd = SpriteController.spriteLoader.Load(Emitter.spriteInfo, Emitter);
        sr.sprite = sd.sprite;
        if(Emitter.spriteInfo.tint != null) {
            sr.color = Emitter.spriteInfo.tint;
        }
        if (sd.layer != null) sr.sortingLayerName = sd.layer;
    }
}
public class EntityComponent : EntityComponent<ConcreteEntity> { }