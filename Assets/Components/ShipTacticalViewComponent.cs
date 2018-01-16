using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTacticalViewComponent : ObserverBehaviour<Ship> {

    private void Start() {
        gameObject.layer = (int) AppInfo.Layer.TacticalMap;

        gameObject.transform.localScale = new Vector3(4, 4, 1);
        gameObject.AddComponent<SpriteRenderer>().sprite = SpriteLoader.Instance.tryLoadSprite("Minimap", "ship").sprite;
    }

    public void MoveTo(Coordinate c) {
        transform.position = c.ToVector();
    }

    override public void HandleEvent(string signal, object[] args) {
        switch(signal) {
            case "MoveTo":
                MoveTo((Coordinate) args[0]);
                break;
        }
    }

    override public void HandleEvent(string signals) {

    }
}
