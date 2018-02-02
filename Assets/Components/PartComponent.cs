using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartComponent : SpritedObserverBehaviour<Part> {
    void Start() {
        //sr = gameObject.AddComponent<SpriteRenderer>();
        //sr.sprite = SpriteController.spriteLoader.GetSprite("part");
        
        BoxCollider2D coll = gameObject.AddComponent<BoxCollider2D>();
        coll.size = new Vector2(1f, 1f);
    }

    void AddAt(Coordinate coordinate) {
        //Debug.Log(String.Format("Adding part at {0}x{1}.",
            //Emitter.position.x, Emitter.position.y));

        transform.localPosition = coordinate.ToVector();

        base.CreateGraphics();

        transform.localEulerAngles = new Vector3(0, 0, 90 * (int)Emitter.Facing);
    }

    override public void HandleEvent(string signal, object[] args) {
        switch(signal) {
            case "AddTo":
                AddAt((Coordinate) args[0]);
                break;
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

    void Update() {
        base.UpdateGraphics();
    }
}
