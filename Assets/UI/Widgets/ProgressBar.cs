using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System;

namespace GUI {
    [MoonSharpUserData]
    public class ProgressBar : Widget {
        Image barComponent;
        Text textComponent;

        float size;
        float factor = 1f;

        public ProgressBar() {
            GameObject imageGameObject = new GameObject("image", new Type[] { typeof(Image) });
            imageGameObject.transform.SetParent(GameObject.transform);
            barComponent = GameObject.GetComponentInChildren<Image>();
            barComponent.sprite = SpriteController.spriteLoader.tryLoadSprite("UI", "button_background");
            imageGameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
            imageGameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);

            int height = 10;
            int leftPadding = 10;
            int rightPadding = 10;
            float value = 0.8f;
            size = imageGameObject.GetComponent<RectTransform>().sizeDelta.x;
            float delta = size * (1 - value);

            imageGameObject.GetComponent<RectTransform>().offsetMin = new Vector2(leftPadding, -height/2);
            imageGameObject.GetComponent<RectTransform>().offsetMax = new Vector2(- rightPadding - delta, height/2);
            //imageGameObject.GetComponent<RectTransform>().hei

            GameObject textGameObject = new GameObject("text", new Type[] { typeof(Text) });
            textGameObject.transform.SetParent(GameObject.transform);
            textComponent = GameObject.GetComponentInChildren<Text>();
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            textComponent.rectTransform.anchorMin = new Vector2(0, 0);
            textComponent.rectTransform.anchorMax = new Vector2(1, 1);
            textComponent.rectTransform.offsetMin = Vector2.zero;
            textComponent.rectTransform.offsetMax = Vector2.zero;
        }
        
        public ProgressBar(string id) : this() {
            Id = id;
        }

        public override object Value {
            set {
                int height = 10;
                int leftPadding = 10;
                int rightPadding = 10;
                float v = Convert.ToSingle(value.ToString());
                float delta = size * (1.0f - v/ factor) ;

                barComponent.rectTransform.offsetMin = new Vector2(leftPadding, -height / 2);
                barComponent.rectTransform.offsetMax = new Vector2(-rightPadding - delta, height / 2);
            }
        }

        public static ProgressBar Create(string id) {
            return new ProgressBar(id);
        }

        public static ProgressBar Create(XmlReader reader, IWidget parent = null) {
            ProgressBar progressBar = new ProgressBar();
            progressBar.ReadElement(reader, parent);
            progressBar.SetParent(parent);
            
            while (reader.Read()) {
                if (reader.NodeType == XmlNodeType.Element) {
                    switch (reader.Name) {
                        case "Sprite":
                            Sprite sprite = SpriteController.spriteLoader.Load(new SpriteInfo(reader));
                            progressBar.barComponent.sprite = sprite;
                            break;
                        case "Value":
                            string argName = "@" + reader.GetAttribute("argument");
                            string propName = reader.ReadElementContentAsString();
                            progressBar.LinkArgNameToValue(argName, propName);
                            break;
                        default:
                            XmlReader subReader = reader.ReadSubtree();
                            GUIController.ReadElement(subReader, progressBar);
                            subReader.Close();
                            break;
                    }
                }
            }
            
            return progressBar;
        }
        internal void ReadElement(XmlReader reader, IWidget parent) {
            string factorString = reader.GetAttribute("factor");
            if (factorString != null) {
                factor = Convert.ToSingle(factorString);
            }

            base.ReadElement(reader, parent);
        }

        public void SetTint(Color color) {
            barComponent.color = color;
        }
    }

}