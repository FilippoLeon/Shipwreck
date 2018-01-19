using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[MoonSharpUserData]
abstract public class ConcreteEntity<T> : Entity<T> where T : class {

    public SpriteInfo spriteInfo;

    private Vector2 position;
    public Vector2 Position {
        get { return position; }
        set {
            position = value;
            Emit("SetPosition", new object[] { position });
        }
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
    }
}

public class ConcreteEntity : ConcreteEntity<ConcreteEntity> { }