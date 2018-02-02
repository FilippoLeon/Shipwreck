using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[MoonSharpUserData]
abstract public class ConcreteEntity<T> : Entity<T>, ISelfDestructible where T : class  {
    
    private Vector2 position;
    public Vector2 Position {
        get { return position; }
        set {
            position = value;
            Emit("SetPosition", new object[] { position });
        }
    }
    

    private float angle;
    public float Angle {
        get { return angle;  }
        set {
            angle = value;
            Emit("SetAngle", new object[] { angle });
        }
    }

    public ConcreteEntity() {
    }

    public ConcreteEntity(ConcreteEntity<T> other) : base(other) {
        spriteInfo = other.spriteInfo;
        position = other.position;
        active = other.active;
        Verse = other.Verse;
    }

    private bool active;
    public bool Active {
        get { return active; }
        set {
            active = value;
            Emit("SetActive", new object[] { active });
        }
    }

    public Verse Verse {set; get;}

    internal bool Spawn(Vector2 position) {
        this.Position = position;

        Emit("SpawnAt", new object[] { position });

        return true;
    }

    public override T Clone() {
        throw new NotImplementedException();
    }

    public override void Update() {
        Emit("OnUpdate", new object[] { this });
    }
}

public class ConcreteEntity : ConcreteEntity<ConcreteEntity> {
    public ConcreteEntity() : base() { }
    public ConcreteEntity(ConcreteEntity other) : base(other) { }
}