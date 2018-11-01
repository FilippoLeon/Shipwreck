using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[MoonSharpUserData]
abstract public class Entity<T> : Emitter<T>, ICloneable<T>, IUpdateable, IXmlSerializable where T : class {
    protected Dictionary<string, object> parameters = new Dictionary<string, object>();
    
    private Direction facing;
    /// <summary>
    /// Where the part is pointing.
    /// </summary>
    public Direction Facing {
        set {
            facing = value;
            Emit("OnFacingChanged");
        }
        get {
            return facing;
        }
    }

    /// <summary>
    /// Rotate the part.
    /// </summary>
    public void Rotate() {
        Facing = (Direction)(((int)Facing + 1) % 4);
    }

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
    
    /// <summary>
    /// Information about the sprite used to represent the part.
    /// </summary>
    public Icon icon;
    public virtual Icon Icon {
        get { return icon; }
        set { icon = value; }
    }

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
        icon = other.icon;

        actions = new Dictionary<string, List<GenericAction>>(other.actions);
    }

    override public void ReadXml(XmlReader reader) {
        base.ReadCurrentElement(reader);

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
                icon = new Icon(subreader, this);
                subreader.Close();
                break;
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
            case "table":
                parameters[name] = new ParameterTable();
                break;
            case null:
                Debug.LogError("Invalid type of parameter.");
                break;
        }
    }

    public override object SetParameter(string name, object value) {
        parameters[name] = value;
        Emit("On" + name + "Changed");
        return value;
    }

    public void SetParameterAsInt(string name, int value) {
        parameters[name] = value;
        Emit("On" + name + "Changed");
    }


    public override object HasParameter(string name) {
		return parameters.ContainsKey(name);
	}
	
    public override object GetParameter(string name) {
        if ( parameters.ContainsKey(name)) {
            return parameters[name];
        } else {
            return null;
        }
    }
    public V GetParameter<V>(string name) where V: new() {
        if (!parameters.ContainsKey(name)) {
            parameters[name] = new V();
        }
        return (V) parameters[name];
    }

    override public void WriteXml(XmlWriter writer) {
        throw new NotImplementedException();
    }

    public abstract T Clone();
    
    public abstract void Update();
}