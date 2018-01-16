using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Xml;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public partial class Part : Entity<Part> {   
    public bool IsRoot { set; get; }

    public Part Hull { set; get; }

    public enum PartType {
        Hull, Addon
    }

    public PartType partType;

    public SpriteInfo spriteInfo;

    public Coordinate position;
    public Coordinate Position {
        get { return position; }
    }

    Ship ship = null;
    Ship Ship {
        set {
            ship = value;
        }
        get {
            return ship;
        }
    }

    int health = 100;
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

    int maxHealth = 100;
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
    int energy = 100;
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
    int energyCapacity = 100;
    public int EnergyCapacity {
        set {
            energyCapacity = value;
            Emit("OnEnergyCapcacityChanged");
            //Ship.RecomputeMaxHealth();
        }
        get {
            return energyCapacity;
        }
    }

    public string Name {
        get { return Id; }
    }

    public List<Part> Neighbours() {
        List<Part> ret = new List<Part>(4);
        ret.Add(Ship.HullAt(position + Coordinate.Up));
        ret.Add(Ship.HullAt(position + Coordinate.Left));
        ret.Add(Ship.HullAt(position + Coordinate.Down));
        ret.Add(Ship.HullAt(position + Coordinate.Right));
        return ret;
    }

    public bool CanAttachTo(Ship ship, Coordinate position) {
        // Check if ship has root
        if (IsRoot) {
            if (ship.Root == null) {
                return true;
            } else {
                return false;
            }
        }

        switch ( partType ) {
            case PartType.Hull:
                List<Part> cparts = ship.PartAt(position);
                if (cparts != null) {
                    foreach (Part p in cparts) {
                        if (p.partType == PartType.Hull) {
                            return false;
                        }
                    }
                }

                bool adjacentHull = false;
                IEnumerable<List<Part>> nhbd = ship.GetNeighbourhoods(position);
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
                List<Part> parts = ship.PartAt(position);
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
        this.Ship = ship;

        Emit("AddTo", new object[] { position });

        return true;
    }

    public Part(XmlReader reader) : this(){
        ReadXml(reader);
    }

    public Part() { }

    private Part(Part other) : base(other) {
        spriteInfo = other.spriteInfo;
        IsRoot = other.IsRoot;
        partType = other.partType;

        health = other.health;
        maxHealth = other.maxHealth;
        energy = other.energy;
        energyCapacity = other.energyCapacity;
    }

    override public void ReadXml(XmlReader reader) {
        base.ReadCurrentElement(reader);

        //Debug.Log(String.Format("New part with id = {0}", Id));

        if ( reader.GetAttribute("root") != null ) {
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
            case "Icon":
                XmlReader subreader = reader.ReadSubtree();
                subreader.ReadToDescendant("Sprite");
                spriteInfo = new SpriteInfo(subreader, this);
                subreader.Close();
                break;
            default:
                base.ReadElement(reader);
                break;
        }
    }

    public override Part Clone() {
        return new Part(this);
    }

    int i = 0;
    public override void Update() {
        SetParameter("direction", ((i++ + 1) % 4).ToString());

        Emit("OnUpdate");
    }
}
