using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

[MoonSharpUserData]
public class EntityRegistry {

    public Dictionary<string, Dictionary<string, ConcreteEntity>> prototypes = new Dictionary<string, Dictionary<string, ConcreteEntity>>();

    public ConcreteEntity Get(string category, string name) {
        if (prototypes.ContainsKey(category)) {
            if (prototypes[category].ContainsKey(name)) {
                return prototypes[category][name].Clone();
            } else {
                Debug.LogError(String.Format("Entity {0} not found in prototypes category {1}.", name, category));
                return null;
            }
        } else {
            Debug.LogError(String.Format("Category {0} not found in prototypes.", category));
            return null;
        }
    }

    public void ReadPrototypes(string pathXml) {
        if (File.Exists(pathXml)) {
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlReader reader = XmlReader.Create(pathXml, settings);

            reader.ReadToFollowing("Prototypes");
            string defaultCategory = reader.GetAttribute("category");
            if( !prototypes.ContainsKey(defaultCategory) ) {
                prototypes[defaultCategory] = new Dictionary<string, ConcreteEntity>();
            }

            int count = 0;
            while( reader.Read() ) {
                switch(reader.Name) {
                    case "Projectile":
                        string id = reader.GetAttribute("id");
                        prototypes[defaultCategory][id] = new Projectile(reader);
                        count++;
                        break;
                    case "Entity":
                        count++;
                        break;
                }
            }

            Debug.Log(String.Format("Loaded {0} Entitiy prototypes.", count));
        } else {
            Debug.LogError(String.Format("File '{0}' not found.", pathXml));
        }
    }
}