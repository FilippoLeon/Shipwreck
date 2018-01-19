using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[MoonSharpUserData]
public class Projectile : ConcreteEntity {
    public struct LocationInfo {
        public Ship ship;
        public Coordinate coordinate;
    }

    public LocationInfo sourceInfo, targetInfo;

    public Projectile() {

    }

    public override void Update() {
        Vector2 dir = (targetInfo.ship.Position + targetInfo.coordinate.ToVector()) - (sourceInfo.ship.Position + sourceInfo.coordinate.ToVector());
        Position += 0.1f * dir.normalized;
    }

    public void Hit(IEmitter emitter) {
        if (emitter is Part) {
            Part hull = (emitter as Part).Ship.HullAt((emitter as Part).Position);
            Part other = targetInfo.ship.HullAt(targetInfo.coordinate) as Part;
            if (hull == other) {
                // Emit damage signal
                hull.Health -= 5;

                Verse.RemoveEntity(this);
                SelfDestroy();

                Debug.Log("BOOOOOOOOOOOOOOOOOOOM!");
            }
        }
    }
}
