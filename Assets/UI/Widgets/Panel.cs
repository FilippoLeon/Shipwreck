using System;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Collections.Generic;
using GUI;

namespace GUI {
    [MoonSharpUserData]
    public class Panel : WidgetContainer {
        UnityEngine.UI.Image background;

        public override void SetValue(object o, int idx) {
            throw new NotImplementedException();
        }
        
        public Panel(string id = null) : base(id) {
            GameObject.AddComponent<CanvasRenderer>();
            background = GameObject.AddComponent<UnityEngine.UI.Image>();
            background.type = UnityEngine.UI.Image.Type.Sliced;
            background.sprite = SpriteController.spriteLoader.tryLoadSprite("UI", "panel_background").sprite;

            SetAnchor(new Vector2(0, 0), new Vector2(1, 0.1f));
            SetSize();
            SetMargin();
        }

        public static Panel Create(string id) {
            return new Panel(id);
        }
        
        public static Panel Create(XmlReader reader, IWidget parent = null) {
            Panel panel = new Panel();
            panel.ReadCurrentElement(reader, parent);
            
            if ( reader.GetAttribute("background") != null ) {
                if (reader.GetAttribute("background") == "none") {
                    GameObject.Destroy(panel.background);
                }
            }

            panel.ReadElements(reader);

            return panel;
        }
    }
}