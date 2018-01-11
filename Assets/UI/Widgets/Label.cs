using System;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Reflection;
using System.Linq;

namespace GUI {
    [MoonSharpUserData]
    public class Label : Widget {
        Text textComponent;

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

        GenericAction addedAction = null;

        public static Label Create(XmlReader reader, IWidget parent = null) {
            Label label = new Label();
            label.ReadElement(reader, parent);
            label.SetParent(parent);

            if (reader.GetAttribute("alignment") != null) {
                label.SetAlignment( (TextAnchor) Enum.Parse( typeof(TextAnchor), reader.GetAttribute("alignment") ) );
            }

            while (reader.Read()) {
                if (reader.NodeType == XmlNodeType.Element) {
                    if (reader.Name == "Text") {
                        string tType = reader.GetAttribute("type");
                        switch(tType) {
                            case "dynamic":
                                string argName = "@" + reader.GetAttribute("argument");
                                label.Text = reader.ReadElementContentAsString();
                                
                                label.textType = TextType.Dynamic;

                                System.Action<object[]> action = (object[] o) => {
                                    IEmitter arg = label.Root.GetArgument(argName);
                                    Debug.Log(label.Text);
                                    Debug.Log(GetPropValue(arg, label.Text).ToString());
                                    label.textComponent.text = GetPropValue(arg, label.Text).ToString();
                                };
                                label.Root.ChangeArguments += () => {
                                    label.Root.GetArgument(argName).RemoveAction("On" + label.Text + "Changed", label.addedAction);
                                    label.addedAction = label.Root.GetArgument(argName).AddAction("On" + label.Text  + "Changed", action);
                                };
                                break;
                            default:
                            case "static":
                                label.textType = TextType.Static;
                                label.Text = reader.ReadElementContentAsString();
                                break;
                        }
                    } else {
                        XmlReader subReader = reader.ReadSubtree();
                        GUIController.ReadElement(subReader, label);
                        subReader.Close();
                    }
                }
            }


            return label;
        }

        public override void Update(object[] args) {
            //textComponent.text = GenerateText();
            base.Update(args);
        }
    }
}