using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galaxy : Entity<Galaxy> {
    Dictionary<string, SolarSystem> systems = new Dictionary<string, SolarSystem>();

    Verse Verse {set; get;}

    public Galaxy(Verse verse) {
        Verse = verse;
    }

    public override Galaxy Clone() {
        throw new System.NotImplementedException();
    }

    public override void Update() {
        throw new System.NotImplementedException();
    }

    internal void Create() {
        int size = 44;

        for(int i = 0; i < size; ++i) {
            string name = Verse.registry.namesRegistry.GetRandom("systems");
            Coordinate coordinate = new Coordinate((int) UnityEngine.Random.Range(-50f,50f), (int) UnityEngine.Random.Range(-50f,50f));

            systems[name] = new SolarSystem(name, this, coordinate);
        }

        Emit("OnCreate", new object[] { });
    }
}
