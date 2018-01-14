using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[MoonSharpUserData]
abstract public class ConcreteEntity<T> : Entity<T> where T : class {

    public SpriteInfo spriteInfo;

    private Coordinate position;
    public Coordinate Position {
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
    
    public bool Spawn(Coordinate position) {
        this.Position = position;

        Emit("SpawnAt", new object[] { position });

        return true;
    }

    public override T Clone() {
        throw new NotImplementedException();
    }
    
    public override void Update() {
        throw new NotImplementedException();
    }
}

public class ConcreteEntity : ConcreteEntity<ConcreteEntity> { }