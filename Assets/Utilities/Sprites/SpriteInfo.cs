using System;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class SpriteInfo {
    public string id;
    public string category;
    public string type;

    public SpriteInfo() {

    }

    public SpriteInfo(SpriteInfo other) {
        this.id = other.id;
        this.type = other.type;
        this.category = other.category;
    }

    public SpriteInfo(XmlReader reader) {
        ReadXml(reader);
    }

    public void ReadXml(XmlReader reader) {

        //Debug.Log("Reading sprite information.");
        string type = reader.GetAttribute("type");
        if (type != null) {
            //Debug.Log("Reading sprite information.");
            this.type = type;
        }

        string spriteId = reader.GetAttribute("id");
        if (spriteId == null) {
            Debug.LogWarning("No id for Sprite!");
        } else {
            this.id = spriteId;
        }
        category = reader.GetAttribute("category");
        //Debug.Log(String.Format("Sprite of {0} has id {1} and type {2}", id, this.id, this.type));
    }
}