using MoonSharp.Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[MoonSharpUserData]
public class Ship : Entity<Ship> {

    Dictionary<Coordinate, List<Part>> parts = new Dictionary<Coordinate, List<Part>>();
    public Part Root { get; set; }

    public Verse verse;

    public Ship(Verse verse) {
        verse.AddShip(this);
        this.verse = verse;
    }

    string name = "Serenity";
    public string Name {
        set {
            name = value;
            Emit("OnNameChanged");
        }
        get {
            return name;
        }
    }

    int health = 0;
    public int Health {
        set {
            health = value;
            Emit("OnHealthChanged");
        }
        get {
            return health;
        }
    }
    int maxHealth = 0;
    public int MaxHealth {
        set {
            maxHealth = value;
            Emit("OnMaxHealthChanged");
        }
        get {
            return maxHealth;
        }
    }

    int energy = 0;
    public int Energy {
        set {
            energy = value;
            Emit("OnEnergyChanged");
        }
        get {
            return energy;
        }
    }
    int energyCapacity = 0;
    public int EnergyCapacity {
        set {
            energyCapacity = value;
            Emit("OnEnergyCapacityChanged");
        }
        get {
            return energyCapacity;
        }
    }

    internal void RecomputeHealth() {
        int h = 0;
        foreach(List<Part> pList in parts.Values) {
            foreach (Part p in pList) {
                h += p.Health;
            }
        }
        Health = h;
    }
    internal void RecomputeMaxHealth() {
        int h = 0;
        foreach (List<Part> pList in parts.Values) {
            foreach (Part p in pList) {
                h += p.MaxHealth;
            }
        }
        MaxHealth = h;
    }


    public List<Part> PartAt(Coordinate position) {
        if( !parts.ContainsKey(position) ) {
            return null;
        }
        return parts[position];
    }

    public void AddPart(Part part, Coordinate position) {

        if (!part.CanAttachTo(this, position)) {
            return;
        }

        Emit("AddPart", new object[] { part });

        part.AddTo(this, position);

        if( part.IsRoot ) {
            Root = part;
        }

        if( !parts.ContainsKey(position) ) {
            parts[position] = new List<Part>();
        }
        parts[position].Add(part);

        RecomputeHealth();
        RecomputeMaxHealth();
    }

    internal IEnumerable<List<Part>> GetNeighbourhoods(Coordinate position) {
        List<List<Part>> ret = new List<List<Part>>();

        ret.Add(PartAt(position + Coordinate.Up));
        ret.Add(PartAt(position + Coordinate.Left));
        ret.Add(PartAt(position + Coordinate.Down));
        ret.Add(PartAt(position + Coordinate.Right));

        return ret;
    }

    public override Ship Clone() {
        throw new System.NotImplementedException();
    }

    public void Update() {
        foreach(List<Part> pl in parts.Values) {
            foreach(Part p in pl) {
                p.Update();
            }
        }
    }
}
