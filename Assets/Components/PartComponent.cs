using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartComponent : ObserverBehaviour<Part> {
    SpriteRenderer sr;
    void Start() {
        //sr = gameObject.AddComponent<SpriteRenderer>();
        //sr.sprite = SpriteController.spriteLoader.GetSprite("part");
    }

    void AddAt(Coordinate coordinate) {
        Debug.Log(String.Format("Adding part at {0}x{1}.",
            Emitter.position.x, Emitter.position.y));

        transform.position = coordinate.ToVector();

        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteController.spriteLoader.Load(Emitter.si);
    }

    override public void HandleEvent(string signal, object[] args) {
        switch(signal) {
            case "AddTo":
                AddAt((Coordinate) args[0]);
                break;
        }
    }

    override public void HandleEvent(string signals) {

    }

    void Update() {
        sr.sprite = SpriteController.spriteLoader.Load(Emitter.si);
    }
}
