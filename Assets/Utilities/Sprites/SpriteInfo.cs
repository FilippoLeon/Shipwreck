using MoonSharp.Interpreter;
using System;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[MoonSharpUserData]
public class SpriteInfo {
    public string id;
    public string category;
    public string type;
    public GenericAction action = null;

    public SpriteInfo() {

    }

    public SpriteInfo(string category, string id) {
        this.id = id;
        this.category = category;
    }

    public SpriteInfo(SpriteInfo other) {
        this.id = other.id;
        this.type = other.type;
        this.category = other.category;
        this.action = other.action;
    }

    public SpriteInfo(XmlReader reader, IEmitter emitter = null) {
        ReadXml(reader, emitter);
    }

    public void ReadXml(XmlReader reader, IEmitter emitter = null) {
        Debug.Assert(reader.Name == "Sprite");

        //Debug.Log("Reading sprite information.");
        category = reader.GetAttribute("category");

        this.type = reader.GetAttribute("type");

        string content = reader.ReadElementContentAsString().Trim();

        switch( type) {
            case "function":
                string id = reader.GetAttribute("id");
                action = new GenericAction(emitter, id + "_sprite", category, content);
                break;
            case "inline":
                throw new NotImplementedException();
            case null:
            default:
                if (content == null) {
                    Debug.LogWarning("No id for Sprite!");
                } else {
                    this.id = content;
                }
                break;
        }
        
        //Debug.Log(String.Format("Sprite of {0} has id {1} and type {2}", id, this.id, this.type));
    }

    internal string GetId(IEmitter obj) {
        switch(type) {
            case "function":
                if(action == null) {
                    Debug.LogError("No action attached to a functional SpriteInfo");
                    return null;
                }
                //try {
                    return action.Call(obj, new object[] { obj }).String;
                //} catch(Exception e)  {
                    //Debug.LogError(e.Message);
                    //return null;
                //}
            case null:
            default:
                return id;
        }
    }
}