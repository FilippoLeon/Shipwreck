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
    public class ScrollView : Widget, IWidgetContainer {
        ScrollRect scrollRect;

        List<IWidget> childs = new List<IWidget>();

        public ScrollView() {
            GameObject scrollView = GameObject.Instantiate(GameObject.FindObjectOfType<GUIPrefabs>().scrollViewPrefab);
            scrollRect = scrollView.GetComponent<ScrollRect>();
        }
        
        public ScrollView(string id) : this() {
            Id = id;
        }

        public IEnumerable<IWidget> Childs {
            get {
                throw new NotImplementedException();
            }
        }

        public static ScrollView Create(string id) {
            return new ScrollView(id);
        }
        
        public static ScrollView Create(XmlReader reader, IWidget parent = null) {
            ScrollView scrollView = new ScrollView();
            scrollView.ReadElement(reader, parent);
            scrollView.SetParent(parent);
            
            while (reader.Read()) {
                if (reader.NodeType == XmlNodeType.Element) {
                    switch (reader.Name) {
                        default:
                            scrollView.ReadSubElement(reader);
                            break;
                    }
                }
            }

            scrollView.FinalizeRead();

            return scrollView;
        }
        

        public override void Update(object[] args) {
            base.Update(args);
        }
    }
}