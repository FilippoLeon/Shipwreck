using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Xml;
using MoonSharp.Interpreter;

/// <summary>
/// Represents any part that can be attached to a ship. 
/// </summary>
[MoonSharpUserData]
public partial class Part : Entity<Part> {
    /// <summary>
    /// Represents the type of the part. An Hull is where all other parts are attached and 
    /// there can be only one hull at a given coordinate. An addon must be attached to an hull.
    /// </summary>
    public enum PartType {
        Hull, Addon
    }

    //////////
    ////// PROPERTIES
    /////////

    /// <summary>
    /// Is the root of the ship to which it is attached.
    /// </summary>
    public bool IsRoot { set; get; }

    /// <summary>
    /// Represents the hull to which the part is attached.
    /// </summary>
    public Part Hull { set; get; }

    /// <summary>
    /// Represent the part type of the current part.
    /// </summary>
    public PartType partType;

    /// <summary>
    /// Information about the sprite used to represent the part.
    /// </summary>
    public SpriteInfo spriteInfo;
    public override SpriteInfo SpriteInfo {
        get { return spriteInfo;  }
        set { spriteInfo = value; }
    }

    public Coordinate position;
    /// <summary>
    /// The coordinate of the part relaive to the ship.
    /// </summary>
    public Coordinate Position {
        get { return position; }
    }

    private Ship ship = null;
    /// <summary>
    /// The ship to which the part is attached.
    /// </summary>
    public Ship Ship {
        protected set {
            ship = value;
        }
        get {
            return ship;
        }
    }

    private int health = 100;
    /// <summary>
    /// The health of the part. Emits "OnHealthChanged" and updates the Ship's health.
    /// </summary>
    public int Health {
        set {
            health = value;
            Emit("OnHealthChanged");
            Ship.RecomputeHealth();
        }
        get {
            return health;
        }
    }

    private int maxHealth = 100;
    public int MaxHealth {
        set {
            maxHealth = value;
            Emit("OnMaxHealthChanged");
            Ship.RecomputeMaxHealth();
        }
        get {
            return maxHealth;
        }
    }
    private int energy = 100;
    public int Energy {
        set {
            energy = value;
            Emit("OnEnergyChanged");
            //Ship.RecomputeMaxHealth();
        }
        get {
            return energy;
        }
    }
    private int _energyCapacity = 100;
    /// <summary>
    /// Contains the entire energy capacity of the part. Emits "OnEnergyCapacityChanged".
    /// </summary>
    public int EnergyCapacity {
        set {
            _energyCapacity = value;
            Emit("OnEnergyCapcacityChanged");
            //Ship.RecomputeMaxHealth();
        }
        get {
            return _energyCapacity;
        }
    }

    public new string Name {
        get { return Id; }
    }

    public int Price { get; internal set; }

    public Part(XmlReader reader) : this() {
        ReadXml(reader);
    }

    public Part() { }

    private Part(Part other) : base(other) {
        spriteInfo = other.spriteInfo;
        IsRoot = other.IsRoot;
        partType = other.partType;
        Price = other.Price;

        health = other.health;
        maxHealth = other.maxHealth;
        energy = other.energy;
        _energyCapacity = other._energyCapacity;
    }

    public override void ReadXml(XmlReader reader) {
        base.ReadCurrentElement(reader);

        //Debug.Log(String.Format("New part with id = {0}", Id));

        if (reader.GetAttribute("root") != null) {
            IsRoot = Convert.ToBoolean(reader.GetAttribute("root"));
        }
        if (reader.GetAttribute("type") != null) {
            switch (reader.GetAttribute("type")) {
                case "hull":
                    partType = PartType.Hull;
                    break;
                case "addon":
                default:
                    partType = PartType.Addon;
                    break;
            }
        }

        if( reader.GetAttribute("price") != null) {
            Price = Convert.ToInt32(reader.GetAttribute("price"));
        } else {
            Price = 100;
        }

        XmlReader subreader = reader.ReadSubtree();
        while (subreader.Read()) {
            if (subreader.NodeType == XmlNodeType.Element) {
                ReadElement(subreader);
            }
        }
        subreader.Close();
    }

    public override void ReadElement(XmlReader reader) {
        switch (reader.Name) {
            default:
                base.ReadElement(reader);
                break;
        }
    }

    public override Part Clone() {
        return new Part(this);
    }

    public List<Part> Neighbours() {
        List<Part> ret = new List<Part>(4) {
            Ship.HullAt(position + Coordinate.Up),
            Ship.HullAt(position + Coordinate.Left),
            Ship.HullAt(position + Coordinate.Down),
            Ship.HullAt(position + Coordinate.Right)
        };
        return ret;
    }

    public bool CanAttachTo(Ship otherShip, Coordinate position) {
        // Check if ship has root
        if (IsRoot) {
            return otherShip.Root == null;
        }

        switch ( partType ) {
            case PartType.Hull:
                List<Part> cparts = otherShip.PartAt(position);
                if (cparts != null) {
                    foreach (Part p in cparts) {
                        if (p.partType == PartType.Hull) {
                            return false;
                        }
                    }
                }

                bool adjacentHull = false;
                IEnumerable<List<Part>> nhbd = otherShip.GetNeighbourhoods(position);
                foreach (List<Part> nList in nhbd) {
                    if (nList == null) {
                        continue;
                    }
                    foreach (Part n in nList) {
                        if (n.partType == PartType.Hull) {
                            adjacentHull = true;
                        }
                    }
                }
                if (adjacentHull == false) {
                    return false;
                }
                break;
            case PartType.Addon:
                List<Part> parts = otherShip.PartAt(position);
                if (parts != null) {
                    foreach (Part p in parts) {
                        if (p.partType == PartType.Hull) {
                            return p.IsAddonCompatible(this);
                        }
                    }
                }
                return false;
        }

        return true;
    }

    public bool IsAddonCompatible(Part addon) {
        return true;
    }

    public bool AddTo(Ship ship, Coordinate position) {
        if( !CanAttachTo(ship, position) ) {
            return false;
        }

        this.position = position;
        Ship = ship;

        Emit("AddTo", new object[] { position });

        return true;
    }
    

    int i = 0;
    public override void Update() {
        SetParameter("direction", ((i++ + 1) % 4).ToString());

        Emit("OnUpdate");
    }
}
