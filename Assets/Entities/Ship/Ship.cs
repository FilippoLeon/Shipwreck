﻿using MoonSharp.Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[MoonSharpUserData]
public class Ship : Entity<Ship> {

    Dictionary<Coordinate, List<Part>> parts = new Dictionary<Coordinate, List<Part>>();
    public Part Root { get; set; }

    Vector2 position;
    public Vector2 Position {
        set {
            position = value;
            Emit("OnPositionChanged", new object[] { position });
        }
        get {
            return position;
        }
    }
    float angle;
    public float Angle {
        set {
            angle = value;
            Emit("OnPositionChanged", new object[] { position });
        }
        get {
            return angle;
        }
    }

    public Vector2 wayPoint;
    bool moving;
    public bool isMoving {
        get { return moving; }
    }

    Dictionary<Coordinate, Part> hulls = new Dictionary<Coordinate, Part>();

    public Verse verse;
    public ILocation location;
    public ILocation Location {
        get { return location; }
    }

    public void WarpTo(ILocation location) {
        this.location = location;

        Emit("OnWarpTo", new object[] { });
        verse.OnWarpTo(location);
    }

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

    public void AddWaypoint(Vector2 coord) {
        wayPoint = coord;
        moving = true;
    }

    public Part HullAt(Coordinate position) {
        if (!hulls.ContainsKey(position)) {
            return null;
        }
        return hulls[position];
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
        if( part.partType == Part.PartType.Hull ) {
            hulls[position] = part;
        } else {
            part.Hull = hulls[position];
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

    float maxSpeed = 0.4f;
    float speed = 0.0f;
    float acceleration = 0.001f;
    float maxAngularSpeed = 5f;
    float angularSpeed = 0f;
    float angularAcceleration = 0.1f;

    public override void Update() {
        if( moving ) {
            Vector3 distance = wayPoint - position;
            float deltaAngle = Vector3.Angle(distance, Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up);
            // If facing correct direction
            if (deltaAngle <= angularSpeed ) {
                float d = 0.5f * speed * speed / acceleration;
                if ( distance.magnitude > d ) {
                    speed += acceleration;
                } else {
                    speed -= acceleration;
                }
                speed = Mathf.Clamp(speed, 0, maxSpeed);
                Position += speed * (wayPoint - position).normalized;
                if ((position - wayPoint).magnitude < speed) {
                    Position = wayPoint;
                    moving = false;
                }
            // Else rotate
            } else {
                float d = 0.5f * angularSpeed * angularSpeed / angularAcceleration;
                if (deltaAngle > d) {
                    angularSpeed += angularAcceleration;
                } else {
                    angularSpeed -= angularAcceleration;
                }
               angularSpeed = Mathf.Clamp(angularSpeed, 0, maxAngularSpeed);
                Angle -= angularSpeed * Mathf.Sign(
                    Vector3.Cross(wayPoint - position, Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up).z
                    );

            }
        }


        foreach (List<Part> pl in parts.Values) {
            foreach(Part p in pl) {
                p.Update();
            }
        }
        UpdateTemperature();
        UpdatePressure();
    }

    public void UpdateTemperature() {
        int rate = 4;

        foreach (Part p in hulls.Values) {
            foreach (Part n in p.Neighbours()) {
                if (n == null) continue;
                int temperature_p = (int)p.GetParameter("temperature");
                int temperature_n = (int)n.GetParameter("temperature");

                if (temperature_p > temperature_n) {
                    int flow_n = Mathf.Min((temperature_p - temperature_n) / 2, rate);
                    p.SetParameterAsInt("temperature", temperature_p - flow_n);
                    n.SetParameterAsInt("temperature", temperature_n + flow_n);
                }
            }
        }
    }

    public void SpawnProjectile(Projectile p, Coordinate coord, Ship target, Coordinate targetCoordinate) {
        p.Name = "Projectile";
        p.targetInfo.ship = target;
        p.targetInfo.coordinate = targetCoordinate;
        p.sourceInfo.ship = this;
        p.sourceInfo.coordinate = coord;

        p.spriteInfo = new SpriteInfo();
        p.spriteInfo.category = "UI";
        p.spriteInfo.id = "red_square";

        Emit("OnSpawnProjectile", new object[] { p });

        verse.SpawnEntity(p, Position + coord.ToVector());
    }

    public void UpdatePressure() {
        int rate = 4;

        foreach(Part p in hulls.Values) {
            foreach(Part n in p.Neighbours()) {
                if (n == null) continue;
                int pressure_p = (int) p.GetParameter("pressure");
                int pressure_n = (int) n.GetParameter("pressure");
                int min_pressure_p = (int) p.GetParameter("min_pressure");
                int max_pressure_n = (int) n.GetParameter("max_pressure");

                if ( pressure_p > pressure_n) {
                    int flow_n = Mathf.Min((pressure_p - pressure_n) / 2, rate);
                    flow_n = Mathf.Min(flow_n, pressure_p - min_pressure_p);
                    flow_n = Mathf.Min(flow_n, max_pressure_n - pressure_n);
                    p.SetParameterAsInt("pressure", pressure_p - flow_n);
                    n.SetParameterAsInt("pressure", pressure_n + flow_n);
                }
            }
        }
    }
}