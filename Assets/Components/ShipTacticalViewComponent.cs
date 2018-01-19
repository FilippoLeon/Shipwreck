using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTacticalViewComponent : ObserverBehaviour<Ship> {

    GameObject wayPointIndicator;
    GameObject trajectoryIndicator;

    private void Start() {
        gameObject.layer = (int) AppInfo.Layer.TacticalMap;

        gameObject.transform.localScale = new Vector3(1, 1, 1);
        gameObject.AddComponent<SpriteRenderer>().sprite = SpriteLoader.Instance.tryLoadSprite("Minimap", "ship").sprite;

        wayPointIndicator = new GameObject("WayPoint");
        wayPointIndicator.transform.SetParent(this.transform);
        wayPointIndicator.layer = (int)AppInfo.Layer.TacticalMap;

        wayPointIndicator.transform.localScale = new Vector3(1, 1, 1);
        wayPointIndicator.AddComponent<SpriteRenderer>().sprite = SpriteLoader.Instance.tryLoadSprite("Minimap", "waypoint").sprite;

        trajectoryIndicator = new GameObject("Trajectory");
        trajectoryIndicator.transform.SetParent(this.transform);
        trajectoryIndicator.layer = (int)AppInfo.Layer.TacticalMap;
        LineRenderer lr = trajectoryIndicator.AddComponent<LineRenderer>();
        
        lr.startWidth = 0.2f;
        lr.endWidth = 0.2f;
        lr.startColor = Color.black;
        lr.endColor = Color.red;
        lr.material = GameObject.FindObjectOfType<MainController>().starmapEdgeMaterial;
    }

    public void MoveTo(Vector2 c) {
        transform.position = c;
        transform.rotation = Quaternion.AngleAxis(Emitter.Angle, Vector3.forward);
    }

    override public void HandleEvent(string signal, object[] args) {
        switch(signal) {
            case "OnPositionChanged":
                MoveTo((Vector2) args[0]);
                break;
        }
    }

    override public void HandleEvent(string signals) {
        base.HandleEvent(signals);
    }

    void Update() {
        wayPointIndicator.transform.rotation = Quaternion.AngleAxis(5, Vector3.forward) * wayPointIndicator.transform.rotation;
        if ( Emitter.isMoving ) {
            wayPointIndicator.transform.position = Emitter.wayPoint;
            trajectoryIndicator.GetComponent<LineRenderer>().SetPositions(
                new Vector3[] {
                     Emitter.Position,
                     Emitter.wayPoint
                }
                );
            wayPointIndicator.SetActive(true);
            trajectoryIndicator.SetActive(true);
        } else {
            wayPointIndicator.SetActive(false);
            trajectoryIndicator.SetActive(false);
        }
    }
}
