using System;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace GUI {
    [MoonSharpUserData]
    public class Image : Widget {
        UnityEngine.UI.Image imageComponent;
        SpriteInfo spriteInfo;

        public Image(string id = null) : base(id) {
            imageComponent = GameObject.AddComponent<UnityEngine.UI.Image> ();
        }

        public static Image Create(string id = null) {
            return new Image(id);
        }

        public Image SetSprite(SpriteInfo info) {
            spriteInfo = info;
            imageComponent.sprite = SpriteController.spriteLoader.Load(spriteInfo).sprite;
            return this;
        }
        
        public static Image Create(XmlReader reader, IWidget parent = null) {
            Image image = new Image();
            image.ReadCurrentElement(reader, parent);
            
            while (reader.Read()) {
                if (reader.NodeType == XmlNodeType.Element) {
                    switch (reader.Name) {
                        case "Sprite":
                            XmlReader subreader = reader.ReadSubtree();
                            image.SetSprite(new SpriteInfo(subreader));
                            subreader.Close();
                            break;
                        default:
                            image.ReadSubElement(reader);
                            break;
                    }
                }
            }

            image.FinalizeRead();

            return image;
        }
        
        public override void Update(object[] args) {
            base.Update(args);
        }
    }
}