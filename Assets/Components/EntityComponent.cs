using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityComponent : ObserverBehaviour<ConcreteEntity> {
    SpriteRenderer sr;

    void Start() {

    }

    void SpawnAt(Coordinate coordinate) {
        transform.localPosition = coordinate.ToVector();

        sr = gameObject.AddComponent<SpriteRenderer>();
        
        SpriteController.spriteLoader.LoadIntoSpriteRenderer(sr, Emitter.spriteInfo, Emitter);
    }

    override public void HandleEvent(string signal, object[] args) {
        switch(signal) {
            case "SpawnAt":
                SpawnAt((Coordinate) args[0]);
                break;
            case "SetActive":
                gameObject.SetActive((bool) args[0]);
                break;
            case "SetPosition":
                transform.localPosition = ((Coordinate) args[0]).ToVector();
                break;
        }
    }

    override public void HandleEvent(string signals) {

    }

    void Update() {
        SpriteLoader.SpriteContainer sd = SpriteController.spriteLoader.Load(Emitter.spriteInfo, Emitter);
        sr.sprite = sd.sprite;
        if (sd.layer != null) sr.sortingLayerName = sd.layer;
    }
}
