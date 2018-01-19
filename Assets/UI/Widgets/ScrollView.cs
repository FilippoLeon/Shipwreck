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
    public class ScrollView : WidgetContainer {
        ScrollRect scrollRect;
        GameObject content;

        public ScrollView() : base(false) {
            GameObject = GameObject.Instantiate(GameObject.FindObjectOfType<GUIPrefabs>().scrollViewPrefab);
            GameObject.name = Id;
            scrollRect = GameObject.GetComponent<ScrollRect>();
            content = scrollRect.content.gameObject;
            layoutComponent = GameObject.AddComponent<LayoutElement>();

            // Make these choosable
            content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            content.AddComponent<VerticalLayoutGroup>().padding = new RectOffset(2,2,2,2);

            scrollRect.GetComponent<Image>().sprite
                = SpriteController.spriteLoader.tryLoadSprite("UI", "panel_background").sprite;
            scrollRect.horizontalScrollbar.handleRect.GetComponent<Image>().sprite
                = SpriteController.spriteLoader.tryLoadSprite("UI", "panel_background").sprite;
            scrollRect.verticalScrollbar.handleRect.GetComponent<Image>().sprite
                = SpriteController.spriteLoader.tryLoadSprite("UI", "panel_background").sprite;

            GameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            GameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            //scrollRect.GetComponent<RectTransform>().off
            GameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            GameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            GameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            GameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            
        }
        
        public ScrollView(string id) : this() {
            Id = id;
        }
        
        public static ScrollView Create(string id) {
            return new ScrollView(id);
        }

        public override GameObject GetContentGameObject() {
            return content;
        }

        public static ScrollView Create(XmlReader reader, IWidget parent = null) {
            ScrollView scrollView = new ScrollView();
            scrollView.ReadCurrentElement(reader, parent);

            scrollView.ReadElements(reader);

            return scrollView;
        }
    }
}