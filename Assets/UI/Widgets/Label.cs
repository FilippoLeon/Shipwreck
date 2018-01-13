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
    public class Label : Widget {
        Text textComponent;

        GenericAction textModifier = null;

        enum TextType {
            Dynamic, Static
        }

        string content;
        TextType textType;
        public string Text {
            get {
                return content;
            }
            set {
                content = value;
                textComponent.text = content;
            }
        }

        public Color TextColor {
            set {
                textComponent.color = value;
            }
            get {
                return textComponent.color;
            }
        }

        public Label() {
            textComponent = GameObject.AddComponent<Text>();
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            textComponent.color = GUIController.DefaultTextColor;
        }

        void SetAlignment(TextAnchor alignment) {
            textComponent.alignment = alignment;
        }

        public Label(string id) : this() {
            Id = id;
        }

        public static Label Create(string id) {
            return new Label(id);
        }
        
        public override void SetValue(object value, int index) {
            values[index] = value;
            if ( textModifier == null ) {
                textComponent.text = values[0].ToString();
            } else {
                try {
                    textComponent.text = textModifier.Call(this, values.Values.ToArray()).String;
                } catch(Exception e) {

                }
            }
        }

        public static Label Create(XmlReader reader, IWidget parent = null) {
            Label label = new Label();
            label.ReadElement(reader, parent);
            label.SetParent(parent);

            if (reader.GetAttribute("alignment") != null) {
                label.SetAlignment( (TextAnchor) Enum.Parse( typeof(TextAnchor), reader.GetAttribute("alignment") ) );
            }
            
            while (reader.Read()) {
                if (reader.NodeType == XmlNodeType.Element) {
                    switch (reader.Name) {
                        case "Text":
                            string tType = reader.GetAttribute("type");
                            switch(tType) {
                                case "dynamic":
                                    label.textType = TextType.Dynamic;

                                    string argName = "@" + reader.GetAttribute("argument");
                                    string propName = reader.ReadElementContentAsString();
                                    label.LinkArgNameToValue(argName, propName, label.valueIndex++);
                                
                                    label.Text = propName;
                                    break;
                                case "function":
                                    label.textModifier = new GenericAction(label, "OnTextChange", "UI", reader.ReadElementContentAsString());
                                    break;
                                case "script":
                                    throw new NotImplementedException();
                                default:
                                case "static":
                                    label.textType = TextType.Static;
                                    label.Text = reader.ReadElementContentAsString();
                                    break;
                            }
                            break;
                        default:
                            label.ReadSubElement(reader);
                            break;
                    }
                }
            }

            label.FinalizeRead();

            return label;
        }
        

        public override void Update(object[] args) {
            //textComponent.text = GenerateText();
            base.Update(args);
        }
    }
}