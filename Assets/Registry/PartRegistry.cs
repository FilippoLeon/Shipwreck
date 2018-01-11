using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

[MoonSharpUserData]
public class PartRegistry {

    public Dictionary<string, Part> prototypes = new Dictionary<string, Part>();

    public Part Get(string name) {
        if( prototypes.ContainsKey(name) ) {
            return prototypes[name].Clone();
        } else {
            Debug.LogError(String.Format("Part {0} not found in prototypes.", name));
            return null;
        }
    }

    public void ReadPrototypes(string pathXml) {
        if (File.Exists(pathXml)) {
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlReader reader = XmlReader.Create(pathXml, settings);

            reader.ReadToFollowing("Prototypes");

            int count = 0;
            while( reader.Read() ) {
                if( reader.Name == "Part") {
                    string id = reader.GetAttribute("id");
                    prototypes[id] = new Part(reader);
                    count++;
                }
            }

            Debug.Log(String.Format("Loaded {0} Part prototypes.", count));
        } else {
            Debug.LogError(String.Format("File '{0}' not found.", pathXml));
        }
    }
}