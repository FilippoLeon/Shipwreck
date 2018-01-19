using System;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Collections.Generic;
using GUI;

namespace GUI {
    [MoonSharpUserData]
    abstract public class WidgetContainer : Widget, IWidgetContainer {
        private LayoutGroup layout;

        Dictionary<string, IWidget> childs = new Dictionary<string, IWidget>();
        public IEnumerable<IWidget> Childs {
            get {
                return childs.Values;
            }
        }

        protected WidgetContainer() { }

        protected WidgetContainer(bool createGameObject) : base(createGameObject) { }
        
        public void SetLayout(string layoutName) {
            switch (layoutName) {
                case "grid":
                    layout = GameObject.AddComponent<GridLayoutGroup>();
                    break;
                case "horizontal":
                    layout = GameObject.AddComponent<HorizontalLayoutGroup>();
                    SetChildExpand(false);
                    break;
                case "vertical":
                    layout = GameObject.AddComponent<VerticalLayoutGroup>();
                    SetChildExpand(false);
                    break;
            }

        }

        public void SetAnchor(Vector2 anchorMin, Vector2 anchorMax) {
            GameObject.GetComponent<RectTransform>().anchorMin = anchorMin;
            GameObject.GetComponent<RectTransform>().anchorMax = anchorMax;
        }

        public void SetSize() {
            GameObject.GetComponent<RectTransform>().offsetMin = new Vector2(50, 10);
            //GameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            GameObject.GetComponent<RectTransform>().offsetMax = new Vector2(-50, -10);
        }
        public void SetSize(float[] size) {
            if(size[0] == -1) {
                size[0] = GameObject.GetComponent<RectTransform>().offsetMin.x;
            }
            if (size[1] == -1) {
                size[1] = GameObject.GetComponent<RectTransform>().offsetMin.y;
            }
            if (size[2] == -1) {
                size[2] = GameObject.GetComponent<RectTransform>().offsetMax.x;
            }
            if (size[3] == -1) {
                size[3] = GameObject.GetComponent<RectTransform>().offsetMax.y;
            }
            GameObject.GetComponent<RectTransform>().offsetMin = new Vector2(size[0], size[1]);
            GameObject.GetComponent<RectTransform>().offsetMax = new Vector2(size[2], size[3]);
        }
        
        public void SetMargin() {
            if (layout is HorizontalOrVerticalLayoutGroup) {
                (layout as HorizontalOrVerticalLayoutGroup).spacing = 10;
            }
        }

        public void AddChild(IWidget child) {
            childs.Add(child.Id, child);
            child.GameObject.transform.SetParent(this.GetContentGameObject().transform);
        }

        public void SetChildExpand(bool expand = true) {
            if (layout is HorizontalOrVerticalLayoutGroup) {
                (layout as HorizontalOrVerticalLayoutGroup).childForceExpandWidth = expand;
            }
        }
        public void SetChildExpand(string mode, Direction dir) {
            if (layout is HorizontalOrVerticalLayoutGroup) {
                bool expand = false;
                switch (mode) {
                    case "expand":
                        expand = true;
                        break;
                }
                switch (dir) {
                    case Direction.Horizontal:
                        (layout as HorizontalOrVerticalLayoutGroup).childForceExpandWidth = expand;
                        break;
                    case Direction.Vertical:
                        (layout as HorizontalOrVerticalLayoutGroup).childForceExpandHeight = expand;
                        break;
                }
            }
        }

        public  enum Direction {  Horizontal, Vertical };

        public void AddContentSizeFitter(ContentSizeFitter.FitMode mode, Direction dir = Direction.Horizontal) {
            ContentSizeFitter fitter = GameObject.AddComponent<ContentSizeFitter>();
            switch (dir) {
                case Direction.Horizontal:
                    fitter.horizontalFit = mode;
                    break;
                case Direction.Vertical:
                    fitter.verticalFit = mode;
                    break;
            }
        }
        private void SetPadding(int[] padding) {
            layout.padding = new RectOffset(padding[0], padding[1], padding[2], padding[3]);
        }

        internal override void ReadCurrentElement(XmlReader reader, IWidget parent) {
            base.ReadCurrentElement(reader, parent);
            
            if (reader.GetAttribute("anchorMin") != null || reader.GetAttribute("anchorMax") != null) {
                Vector2 anchorMin = XmlUtilities.ToVector2(reader.GetAttribute("anchorMin"));
                Vector2 anchorMax = XmlUtilities.ToVector2(reader.GetAttribute("anchorMax"));
                SetAnchor(anchorMin, anchorMax);
            }
            string layoutstr = reader.GetAttribute("layout");

            Direction dir = Direction.Horizontal;
            SetLayout(layoutstr);
            switch (layoutstr) {
                case "grid":
                    if (reader.GetAttribute("gridX") != null) {
                        (layout as GridLayoutGroup).constraint = GridLayoutGroup.Constraint.FixedRowCount;
                        (layout as GridLayoutGroup).constraintCount = Convert.ToInt32(reader.GetAttribute("gridX"));
                    } else if (reader.GetAttribute("gridY") != null) {
                        (layout as GridLayoutGroup).constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                        (layout as GridLayoutGroup).constraintCount = Convert.ToInt32(reader.GetAttribute("gridY"));
                    }
                    if (reader.GetAttribute("cellSize") != null) {
                        Vector2 cellSize = XmlUtilities.ToVector2(reader.GetAttribute("cellSize"));
                        (layout as GridLayoutGroup).cellSize = cellSize;
                    }
                    break;
                case "horizontal":
                    dir = Direction.Horizontal;
                    break;
                case "vertical":
                    dir = Direction.Vertical;
                    break;
            }

            string childBehaviour = reader.GetAttribute("child");
            if (childBehaviour != null) {
                string[] s = childBehaviour.Split(',');
                SetChildExpand(s[0], Direction.Horizontal);
                SetChildExpand(s[1], Direction.Vertical);
            }

            string contentPolicy = reader.GetAttribute("content");
            if (contentPolicy == null) {

            } else if (contentPolicy.Split(',').Length == 2) {
                AddContentSizeFitter(ToFitMode(contentPolicy.Split(',')[0]), Direction.Horizontal);
                AddContentSizeFitter(ToFitMode(contentPolicy.Split(',')[1]), Direction.Vertical);
            } else {
                switch (contentPolicy) {
                    case "minFit":
                        AddContentSizeFitter(ContentSizeFitter.FitMode.MinSize);
                        break;
                    case "preferredFit":
                        AddContentSizeFitter(ContentSizeFitter.FitMode.PreferredSize, dir);
                        break;
                    case "expand":
                        SetChildExpand(true);
                        break;
                    default:
                        break;
                }
            }


            if (reader.GetAttribute("padding") != null) {
                int[] padding = XmlUtilities.ToIntArray(reader.GetAttribute("padding"));
                SetPadding(padding);
            }
            if (reader.GetAttribute("offset") != null) {
                float[] padding = XmlUtilities.ToFloatArray(reader.GetAttribute("offset"));
                SetSize(padding);
            }
            
        }

        protected void ReadElements(XmlReader reader) {

            while (reader.Read()) {
                if (reader.NodeType == XmlNodeType.Element) {
                    XmlReader subReader = reader.ReadSubtree();
                    GUIController.ReadElement(subReader, this);
                    subReader.Close();
                }
            }
        }

        private static ContentSizeFitter.FitMode ToFitMode(string contentPolicy) {
            switch (contentPolicy) {
                case "minFit":
                    return ContentSizeFitter.FitMode.MinSize;
                case "preferredFit":
                    return ContentSizeFitter.FitMode.PreferredSize;
                default:
                    return ContentSizeFitter.FitMode.Unconstrained;
            }
        }

        public override void Update(object[] aregs) {
            foreach (IWidget child in childs.Values) {
                child.Update(null);
            }
            base.Update(null);
        }

        public IWidget this[string key] {
            get {
                return childs[key];
            }
            set {
                childs[key] = value;
            }
        }
    }
}