using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : Entity<SolarSystem>, IView {
    public Coordinate coordinate;
    private Galaxy galaxy;

    public List<SolarSystem> links = new List<SolarSystem>();

    public List<ILocation> planets = new List<ILocation>();

    public SolarSystem(string name, Galaxy galaxy, Coordinate coordinate) {
        Name = name;
        this.galaxy = galaxy;
        this.coordinate = coordinate;

        int nplanets = UnityEngine.Random.Range(1, 7);
        for( int i = 0; i < nplanets; ++i ) {
            planets.Add(Planet.Random(this, i));
        }
    }

    public string Name {
        get; set;
    }

    public override SolarSystem Clone() {
        throw new System.NotImplementedException();
    }

    public override void Update() {
        throw new System.NotImplementedException();
    }

    public Coordinate GetMin() {
        return new Coordinate(-10, -10);
    }

    public Coordinate GetMax() {
        return new Coordinate(10, 10);
    }
}
