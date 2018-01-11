using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[MoonSharpUserData]
abstract public class Entity<T> : Emitter<T>, ICloneable<T>, IXmlSerializable where T: class {

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

                string type = "string";
                if ( reader.GetAttribute("type") != null ) {
                    type = reader.GetAttribute("type");
                }

                switch( type ) {
                    case "string":
                        parameters.Add(name, reader.ReadElementContentAsString());
                        break;
                    case "int":
                        parameters.Add(name, reader.ReadElementContentAsInt());
                        break;
                    case "float":
                        parameters.Add(name, reader.ReadElementContentAsFloat());
                        break;
                    default:
                        Debug.LogWarning("Unknown Type.");
                        break;
                }
                break;
            default:
                base.ReadElement(reader);
                break;
        }
    }

    override public void WriteXml(XmlWriter writer) {
        throw new NotImplementedException();
    }

    public abstract T Clone();

    public Entity() {

    }

    protected Entity(Entity<T> other) {
        id = other.id;
        parameters = new Dictionary<string, object>(other.parameters);
    }
}