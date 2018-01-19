using MoonSharp.Interpreter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[MoonSharpUserData]
public class Planet : Entity<Planet>, ILocation {
    public List<INpc> npcs = new List<INpc>();

    public SolarSystem solarSystem;

    public SolarSystem SolarSystem {
        get { return solarSystem;  }
    }

    public struct OrbitInfo {
        public float radius { set; get; }
        public float eccentricity { set; get; }
    }

    public OrbitInfo orbitInfo;
    public OrbitInfo Orbit {
        get { return orbitInfo;  }
    }

    public Planet(SolarSystem system) {
        solarSystem = system;
    }

    public static Planet Random(SolarSystem system, int n = -1) {
        Planet planet = new Planet(system);

        planet.Name = system.Name + (n >= 0 ? " " + (char)('a' + n) : "");
        planet.orbitInfo.radius = UnityEngine.Random.Range(1f, 10f);
        planet.orbitInfo.eccentricity = UnityEngine.Random.Range(0.5f, 0.7f);

        return planet;
    }

    public override Planet Clone() {
        throw new System.NotImplementedException();
    }

    public override void Update() {
        throw new System.NotImplementedException();
    }
}
