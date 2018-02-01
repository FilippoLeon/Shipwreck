using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System;
using System.Linq;

namespace GUI {
    [MoonSharpUserData]
    public class Button : Widget {
        UnityEngine.UI.Image backgroundComponent;
        UnityEngine.UI.Button buttonComponent;
        Text textComponent;

        public string Text {
            get {
                return textComponent.text;
            }
            set {
                textComponent.text = value;
            }
        }
        public Color TextColor {
            get {
                return textComponent.color;
            }
            set {
                textComponent.color = value;
            }
        }
        
        public void SetSprite(SpriteInfo info) {
            if (info == null) backgroundComponent.sprite = null;
            else backgroundComponent.sprite = SpriteController.spriteLoader.Load(info).sprite;
        }

        public void SetType(string type) {
            switch (type) {
                case "sliced":
                default:
                    backgroundComponent.type = UnityEngine.UI.Image.Type.Sliced;
                    break;
                case "simple":
                    backgroundComponent.type = UnityEngine.UI.Image.Type.Simple;
                    backgroundComponent.preserveAspect = true;
                    break;
            }
        }

        public void SetColor(Color color) {
            backgroundComponent.color = color;
        }

        public Button(string id = null) : base(id) {
            backgroundComponent = GameObject.AddComponent<UnityEngine.UI.Image>();
            backgroundComponent.type = UnityEngine.UI.Image.Type.Sliced;
            backgroundComponent.sprite = SpriteController.spriteLoader.tryLoadSprite("UI", "button_background").sprite;
            backgroundComponent.SetNativeSize();

            buttonComponent = GameObject.AddComponent<UnityEngine.UI.Button>();

            GameObject textGameObject = new GameObject("text", new Type[] { typeof(Text) });
            textGameObject.transform.SetParent(GameObject.transform);
            textComponent = GameObject.GetComponentInChildren<Text>();
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            textComponent.rectTransform.anchorMin = new Vector2(0, 0);
            textComponent.rectTransform.anchorMax = new Vector2(1, 1);
            textComponent.rectTransform.offsetMin = Vector2.zero;
            textComponent.rectTransform.offsetMax = Vector2.zero;

            SetNonExpanding();
        }

        public override void SetNonExpanding(int minWidth = -1) {
            if ( backgroundComponent.sprite ) {
                base.SetNonExpanding((int) backgroundComponent.sprite.rect.width);
            }
        }

        public override GenericAction OnActionAdded( string type, GenericAction action ) {
            switch( type ) {
                case "OnClick":
                    buttonComponent.onClick.AddListener(
                        () => {
                            //Debug.Log(values);
                            object[] param = new object[] { GUIController.childs, Verse.Instance, this };
                            action.Call(this, param.Concat(values.Values).ToArray() );
                            }
                        );
                    break;
            }
            return base.OnActionAdded(type, action);
        }
        
        public void OnClick(Closure callback) {
            buttonComponent.onClick.AddListener(() => callback.Call());
        }

        public static Button Create(string id) {
            return new Button(id);
        }

        public static Button Create(XmlReader reader, IWidget parent = null) {
            Button button = new Button();
            button.ReadCurrentElement(reader, parent);

            String type = reader.GetAttribute("type");

            while (reader.Read()) {
                if (reader.NodeType == XmlNodeType.Element) {
                    switch (reader.Name) {
                        case "Text":
                            button.Text = reader.ReadElementContentAsString();
                            break;
                        case "Sprite":
                            Sprite sprite = SpriteController.spriteLoader.Load(new SpriteInfo(reader)).sprite;
                            button.backgroundComponent.sprite = sprite;
                            button.SetNonExpanding();
                            break;
                        default:
                            button.ReadSubElement(reader);
                            break;
                    }
                }
            }
            button.SetType(type);
            button.FinalizeRead();

            return button;
        }

        public void SetPreserveAspect(bool yn = true) {
            backgroundComponent.preserveAspect = yn;
        }

        public void SetTint(Color color) {
            backgroundComponent.color = color;
        }

        public void SetColorBlock(Color normal, Color pressed, Color disabled) {
            ColorBlock block = new ColorBlock();
            block.normalColor = normal;
            block.pressedColor = pressed;
            block.disabledColor = disabled;
            buttonComponent.colors = block;
        }
    }

}