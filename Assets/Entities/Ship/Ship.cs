﻿using MoonSharp.Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Ship with all it's internal data.
/// </summary>
[MoonSharpUserData]
public class Ship : Entity<Ship> {
    /////////
    ////// BASIC PARAMETERS
    /////////

    /// <summary>
    /// The crew of the ship. Includes players and merchants.
    /// </summary>
    public List<ILivingEntity> crew = new List<ILivingEntity>();

	/// <summary>The root of the ship, i.e. the Part to which all other parts are attached.</summary>
	/// <todo>Is this really needed?</todo>
    public Part Root { get; set; }
	
    private Vector2 position;
	/// <summary>Current position in space of the Ship. Emits "OnPositionChanged".</summary>
	public Vector2 Position {
        set {
            position = value;
            Emit("OnPositionChanged", new object[] { position });
        }
        get {
            return position;
        }
    }
    
	private float angle;
    /// <summary>Current angle the ship is pointing at. Emits "OnPositionChanged".</summary>
	public float Angle {
        set {
            angle = value;
            Emit("OnPositionChanged", new object[] { position });
        }
        get {
            return angle;
        }
    }

	/// <summary>
	/// The location towards which the Ship is moving, namely the next waypoint.
	/// This might be invalid if <c>moving == false</c>.
	/// </summary>
    public Vector2 wayPoint;

    private bool moving;
	/// <summary>
	/// The values represents whether the Ship is currently moving or not.
	/// </summary>
    public bool isMoving {
        get { return moving; }
    }

	/// <summary>The universe onto which the Ship lives.</summary>
	public Verse verse;
    
	private ILocation location;
    /// <summary>The current location of the ship, might be orbiting a sun, in outer space, around a planet, ...</summary>
	public ILocation Location {
        get { return location; }
    }
	
	/////////
	////// EXTRA PARAMETERS
	/////////
	
    string name = "Serenity";
	/// <summary>Ship's name. Emits "OnNameChanged".</summary>
    public new string Name {
        set {
            name = value;
            Emit("OnNameChanged");
        }
        get {
            return name;
        }
    }

    internal void RemovePart(Part part) {
        if( part.partType == Part.PartType.Addon) {
            parts[part.position].Remove(part);
        } else {
            foreach (Part p in parts[part.position]) {
                if (p != part) {
                    p.SelfDestroy();
                }
            }
            parts[part.position].Clear();
            parts.Remove(part.position);
            hulls.Remove(part.position);
        }
        if( part.IsRoot) {
            // FIND NEW ROOT
        }

        dirtyHull = true;
    }

    /// <summary>
    /// Set to true to initialize a search of unconnected parts at the next update. This is set to true when a part is removed.
    /// </summary>
    private bool dirtyHull = false;
    /// IDEA FOR THESE PARAMETERS: we could use a setter that simply invalidates the value. And a getter that does the computation
    // whenever the value is invalid.
    // Or we simply keep track of the data as it changes.

    int health = 0;
	/// <summary>Ship's health, this should be updated whenever the Health of a part changes. Emits "OnHealthChanged".</summary>
    public int Health {
        set {
            health = value;
            Emit("OnHealthChanged");
        }
        get {
            return health;
        }
    }
	
	/// <summary>Ship's max health, this should be updated whenever the MaxHealth of a part changes. Emits "OnMaxHealthChanged".
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
    /// <summary>Ships' energy, this should be updated whenever the energy of a single part changes. Emits "OnEnergyChanged".</summary>
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
	/// <summary>Ships energy capacity. This should be updated whenever the ship total energy capacity changes. Emits "OnEnergyCapacityChanged".</summary>
    public int EnergyCapacity {
        set {
            energyCapacity = value;
            Emit("OnEnergyCapacityChanged");
        }
        get {
            return energyCapacity;
        }
    }
	
	/////////
	////// INTERNAL DATA
	/////////
	
	/// <summary>Contains all the parts of the Ship, indexed by coordinate and then by insertion order.</summary>
	/// <todo>Change to MultiDictionary or change hierarchy to be less cumbersome.</todo>
    protected Dictionary<Coordinate, List<Part>> parts = new Dictionary<Coordinate, List<Part>>();

	/// <summary>Quick access to hulls, indexed by Coordinate</summary>
    protected Dictionary<Coordinate, Part> hulls = new Dictionary<Coordinate, Part>();

	/////////
	////// CONSTRUCTORS
	/////////
	
    /// <summary>
    /// Constructs a random ship.
    /// </summary>
    /// <param name="verse">The verse on which the ship will live.</param>
    /// <param name="score">The ship's power (more or less).</param>
    /// <returns>A brand new random ship.</returns>
    public static Ship Random(Verse verse, int score) {
        Ship ship = new Ship(verse);
        ship.Name = verse.registry.namesRegistry.GetRandom("ships");

        return ship;
    }

    /// <summary>
    /// Finds detached hulls and detach them from the Ship.
    /// </summary>
    public void FindDetachedHulls() {
        Coordinate start = Root.position;
        Queue<Coordinate> touched = new Queue<Coordinate>();
        HashSet<Coordinate> connected = new HashSet<Coordinate>();
        touched.Enqueue(start);
        while(touched.Count != 0) {
            Coordinate next = touched.Dequeue();
            connected.Add(next);
            foreach( Part hull in GetNeighbouringHulls(next) ) {
                if( hull != null && !connected.Contains(hull.position) ) {
                    touched.Enqueue(hull.position);
                }
            }
        }

        List<Part> nonConnected = new List<Part>();
        foreach(Part hull in hulls.Values) {
            if (!connected.Contains(hull.position)) {
                nonConnected.Add(hull);
            }
        }

        foreach(Part hull in nonConnected) {
            //hulls.Remove(hull.position);
            hull.SelfDestroy();
        }
        nonConnected.Clear();

        dirtyHull = false;
    }

	/// <summary>Build a ship and set a Universe for the ship.</summary>
    public Ship(Verse verse) {
        verse.AddShip(this);
        this.verse = verse;
    }

	/// <summary>Returns a copy of this ship.</summary>
    public override Ship Clone() {
        throw new System.NotImplementedException();
    }

	/////////
	////// GETTERS
	/////////
	
	/// <summary>Returns the hull at the given coordinate.</summary>
    public Part HullAt(Coordinate position) {
        if (!hulls.ContainsKey(position)) {
            return null;
        }
        return hulls[position];
    }

	/// <summary>Returns the part list at the given coordinate.</summary>
    public List<Part> PartAt(Coordinate position) {
        if( !parts.ContainsKey(position) ) {
            return null;
        }
        return parts[position];
    }

    /// <summary>Returns all neighbours of the part (list of), in the order Up, Left, Down, Right.</summary> 
    internal IEnumerable<Part> GetNeighbouringHulls(Coordinate position) {
        List<Part> ret = new List<Part>();

        ret.Add(HullAt(position + Coordinate.Up));
        ret.Add(HullAt(position + Coordinate.Left));
        ret.Add(HullAt(position + Coordinate.Down));
        ret.Add(HullAt(position + Coordinate.Right));

        return ret;
    }

    /// <summary>Returns all neighbours of the part (list of), in the order Up, Left, Down, Right.</summary> 
    internal IEnumerable<List<Part>> GetNeighbourhoods(Coordinate position) {
        List<List<Part>> ret = new List<List<Part>>();

        ret.Add(PartAt(position + Coordinate.Up));
        ret.Add(PartAt(position + Coordinate.Left));
        ret.Add(PartAt(position + Coordinate.Down));
        ret.Add(PartAt(position + Coordinate.Right));

        return ret;
    }
	
	/////////
	////// GENERIC MEMBERS
	/////////

	/// <summary>Change the ship location to be somewhere else. Emits "OnWarpTo".</summary>
    public void WarpTo(ILocation location) {
		// TODO: maybe we should set position to zero.
		// TODO: maybe switch WarpTo emission onto location change.
        this.location = location;

        Emit("OnWarpTo", new object[] { });
        verse.OnWarpTo(location);
    }

	/// <summary>Triggers a recomputation of the total health.</summary>
    internal void RecomputeHealth() {
        int h = 0;
        foreach(List<Part> pList in parts.Values) {
            foreach (Part p in pList) {
                h += p.Health;
            }
        }
        Health = h;
    }
	
	/// <summary>Triggers a recomputation of the total max health.</summary>
    internal void RecomputeMaxHealth() {
        int h = 0;
        foreach (List<Part> pList in parts.Values) {
            foreach (Part p in pList) {
                h += p.MaxHealth;
            }
        }
        MaxHealth = h;
    }

	/// <summary>Adds a new Waypoint to the ship's journey.</summary>
    public void AddWaypoint(Vector2 coord) {
        wayPoint = coord;
        moving = true;
    }

	/// Adds a part to the given coordinate. Checks if part can be added. Emits "AddPart".</summary>
    public int AddPart(Part part, Coordinate position) {
		// TODO: change to "OnAddPart".
        if (!part.CanAttachTo(this, position)) {
            return 0;
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

        return part.GetParameter<int>("score_cost");
    }

	/// <summary> Spawn a new projectile.</summary>
	/// <param name="p">The projectile to Spawn.</param>
	/// <param name="coord">The relative coordinate of where in the ship the projectile will spawn.</param>
	/// <param name="target">The target to which the peojectile is aimed at.</param>
	/// <param name="targetCoordinate">The coordinate within the target.</param>
    public void SpawnProjectile(Projectile p, Coordinate coord, Ship target, Coordinate targetCoordinate) {
        p.Name = "Projectile";
        p.targetInfo.ship = target;
        p.targetInfo.coordinate = targetCoordinate;
        p.sourceInfo.ship = this;
        p.sourceInfo.coordinate = coord;

        //p.spriteInfo = new SpriteInfo();
        //p.spriteInfo.category = "UI";
        //p.spriteInfo.id = "red_square";

        Emit("OnSpawnProjectile", new object[] { p });

        verse.SpawnEntity(p, Position + coord.ToVector());
    }

	/// <summary>Maximum speed allowed by the craft. (Speed of light?)</summary>
    private float maxSpeed = 0.4f;
	/// <summary>Current speed.</summary>
    private float speed = 0.0f;
	/// <summary>Current acceleration (this shall be constant for now to decelerate in time).</summary>
    private float acceleration = 0.001f;
	/// <summary>The maximal angular speed of the craft.</summary>
    private float maxAngularSpeed = 5f;
	/// <summary>The current angular speed of the craft.</summary>
    private float angularSpeed = 0f;
	/// <summary>The current angular acceleration of the craft.</summary>
    private float angularAcceleration = 0.1f;
	
	/////////
	////// UPDATING METHODS
	/////////

	/// <summary> Update movement, pressure and temperature.</summary>
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
            if(pl == null) {
                continue;
            }
            foreach(Part p in pl) {
                p.Update();
            }
        }
        if(dirtyHull) {
            FindDetachedHulls();
        }

        UpdateTemperature();
        UpdatePressure();
    }

	/// <summary> Evolves internal temperature of the ship.</summary>
    public void UpdateTemperature() {
        int rate = 4;

        foreach (Part p in hulls.Values) {
            foreach (Part n in p.Neighbours()) {
                if (n == null) continue;
                int temperature_p = p.GetParameter<int>("temperature");
                int temperature_n = n.GetParameter<int>("temperature");

                if (temperature_p > temperature_n) {
                    int flow_n = Mathf.Min((temperature_p - temperature_n) / 2, rate);
                    p.SetParameterAsInt("temperature", temperature_p - flow_n);
                    n.SetParameterAsInt("temperature", temperature_n + flow_n);
                }
            }
        }
    }

	/// <summary> Evolves internal pressure of the ship.</summary>
    public void UpdatePressure() {
        int rate = 4;

        foreach(Part p in hulls.Values) {
            foreach(Part n in p.Neighbours()) {
                if (n == null) continue;
                int pressure_p = p.GetParameter<int>("pressure");
                int pressure_n = n.GetParameter<int>("pressure");
                int min_pressure_p = p.GetParameter<int>("min_pressure");
                int max_pressure_n = n.GetParameter<int>("max_pressure");

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
