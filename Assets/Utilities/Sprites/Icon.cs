using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[MoonSharpUserData]
public class Icon {
    private List<SpriteInfo> layers = new List<SpriteInfo>();

    public Icon() {

    }

    public Icon(Icon other) {
        layers = other.layers.ConvertAll(layer => new SpriteInfo(layer));
    }

    public Icon(XmlReader reader, IEmitter emitter = null) {
        ReadXml(reader, emitter);
    }

    public Icon(SpriteInfo[] spriteInfo) {
        layers = spriteInfo.ToList<SpriteInfo>();
    }

    public Icon(SpriteInfo spriteInfo) {
        this.layers.Add(spriteInfo);
    }

    public void ReadXml(XmlReader reader, IEmitter emitter = null) {
        reader.Read();
        Debug.Assert(reader.Name == "Icon");
        
        while(reader.Read()) {
            if( reader.NodeType == XmlNodeType.Element ) {
                switch( reader.Name ) {
                    case "Sprite":
                        XmlReader subreader = reader.ReadSubtree();
                        subreader.Read();
                        layers.Add(new SpriteInfo(subreader, emitter));
                        subreader.Close();
                        break;
                }
            }
        }
    }

    public int Count {
        get { return layers.Count; }
    }

    public SpriteInfo Get(int i) {
        return layers[i];
    }
}