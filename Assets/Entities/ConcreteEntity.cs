using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[MoonSharpUserData]
abstract public class ConcreteEntity<T> : Entity<T> where T : class {

    public SpriteInfo spriteInfo;

    public Coordinate position;
    public Coordinate Position {
        get { return position; }
    }

    public override T Clone() {
        throw new NotImplementedException();
    }
    
    public override void Update() {
        throw new NotImplementedException();
    }
}

public class ConcreteEntity : ConcreteEntity<ConcreteEntity> { }