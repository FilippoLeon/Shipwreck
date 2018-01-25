﻿using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[MoonSharpUserData]
abstract public class Entity<T> : Emitter<T>, ICloneable<T>, IUpdateable, IXmlSerializable where T : class {
    protected Dictionary<string, object> parameters = new Dictionary<string, object>();

    public override string Category {
        get {
            return "Entity";
        }
    }

    string id;
    override public string Id {
        get {
            return id;
        }
        set {
            id = value;
        }
    }

    public string Name { get; set; }

    public virtual SpriteInfo SpriteInfo { get; set; }

    public Entity() {

    }

    protected Entity(Entity<T> other) {
        id = other.id;
        parameters = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> o in other.parameters) {
            if (o.Value is int) {
                parameters[o.Key] = (int)o.Value;
            } else if (o.Value is string) {
                parameters[o.Key] = (string)o.Value;
            } else if (o.Value is float) {
                parameters[o.Key] = (float)o.Value;
            }
        }

        actions = new Dictionary<string, List<GenericAction>>(other.actions);
    }

    override public void ReadXml(XmlReader reader) {
        base.ReadCurrentElement(reader);

        while (reader.Read()) {
            if (reader.NodeType == XmlNodeType.Element) {
                XmlReader subreader = reader.ReadSubtree();
                ReadElement(subreader);
                subreader.Close();
            }
        }
   }

    public override void ReadElement(XmlReader reader) {
        switch (reader.Name) {
            case "Parameter":
                string name = reader.GetAttribute("name");

                string type = null;
                if ( reader.GetAttribute("type") != null ) {
                    type = reader.GetAttribute("type");
                }
                
                AddParameter(name, type, reader.ReadElementContentAsString());
                break;
            default:
                base.ReadElement(reader);
                break;
        }
    }

    private void AddParameter(string name, string type, string value) {
        switch( type ) {
            case "int":
                parameters[name] = Convert.ToInt32(value);
                break;
            case "string":
                parameters[name] = value;
                break;
            case "float":
                parameters[name] = Convert.ToSingle(value);
                break;
            case null:
                Debug.LogError("Invalid type of parameter.");
                break;
        }
    }

    public override void SetParameter(string name, object value) {
        parameters[name] = value;
        Emit("On" + name + "Changed");
    }

    public void SetParameterAsInt(string name, int value) {
        parameters[name] = value;
        Emit("On" + name + "Changed");
    }

    public override object GetParameter(string name) {
        if ( parameters.ContainsKey(name)) {
            return parameters[name];
        } else {
            return null;
        }
    }

    override public void WriteXml(XmlWriter writer) {
        throw new NotImplementedException();
    }

    public abstract T Clone();
    
    public abstract void Update();
}