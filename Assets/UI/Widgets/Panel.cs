using System;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Collections.Generic;
using GUI;

namespace GUI {
    [MoonSharpUserData]
    public class Panel : Widget, IWidgetContainer {
        Image background;

        private LayoutGroup layout;

        List<IWidget> childs = new List<IWidget>();
        public IEnumerable<IWidget> Childs {
            get {
                return childs;
            }
        }

        public override void SetValue(object o, int idx) {
            throw new NotImplementedException();
        }

        public Panel() {
            GameObject.AddComponent<CanvasRenderer>();
            background = GameObject.AddComponent<Image>();
            background.type = Image.Type.Sliced;
            background.sprite = SpriteController.spriteLoader.tryLoadSprite("UI", "panel_background").sprite;

            SetAnchor(new Vector2(0, 0), new Vector2(1, 0.1f));
            SetSize();
            SetMargin();
        }

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

        public void SetMargin() {
            if (layout is HorizontalOrVerticalLayoutGroup) {
                (layout as HorizontalOrVerticalLayoutGroup).spacing = 10;
            }
        }

        public void Add(IWidget child) {
            childs.Add(child);
            child.GameObject.transform.SetParent(GameObject.transform);
        }
        public Panel(string id) : this() {
            this.Id = id;
        }

        public static Panel Create(string id) {
            return new Panel(id);
        }

        public void SetChildExpand(bool expand = true) {
            if (layout is HorizontalOrVerticalLayoutGroup) {
                (layout as HorizontalOrVerticalLayoutGroup).childForceExpandWidth = expand;
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

        public static Panel Create(XmlReader reader, IWidget parent = null) {
            Panel panel = new Panel();
            if (reader.GetAttribute("id") != null) {
                panel.Id = reader.GetAttribute("id");
            }
            panel.SetParent(parent);

            if (reader.GetAttribute("anchorMin") != null || reader.GetAttribute("anchorMax") != null) {
                Vector2 anchorMin = XmlUtilities.ToVector2(reader.GetAttribute("anchorMin"));
                Vector2 anchorMax = XmlUtilities.ToVector2(reader.GetAttribute("anchorMax"));
                panel.SetAnchor(anchorMin, anchorMax);
            }
            string layout = reader.GetAttribute("layout");

            Direction dir = Direction.Horizontal;
            panel.SetLayout(layout);
            switch (layout) {
                case "grid":
                    if (reader.GetAttribute("gridX") != null) {
                        (panel.layout as GridLayoutGroup).constraint = GridLayoutGroup.Constraint.FixedRowCount;
                        (panel.layout as GridLayoutGroup).constraintCount = Convert.ToInt32(reader.GetAttribute("gridX"));
                    } else if (reader.GetAttribute("gridY") != null) {
                        (panel.layout as GridLayoutGroup).constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                        (panel.layout as GridLayoutGroup).constraintCount = Convert.ToInt32(reader.GetAttribute("gridY"));
                    }
                    if (reader.GetAttribute("cellSize") != null) {
                        Vector2 cellSize = XmlUtilities.ToVector2(reader.GetAttribute("cellSize"));
                        (panel.layout as GridLayoutGroup).cellSize = cellSize;
                    }
                    break;
                case "horizontal":
                    dir = Direction.Horizontal;
                    break;
                case "vertical":
                    dir = Direction.Vertical;
                    break;
            }

            string contentPolicy = reader.GetAttribute("content");
            if (contentPolicy == null ) {

            } else if ( contentPolicy.Split(',').Length == 2 ) {
                panel.AddContentSizeFitter(ToFitMode(contentPolicy.Split(',')[0]), Direction.Horizontal);
                panel.AddContentSizeFitter(ToFitMode(contentPolicy.Split(',')[1]), Direction.Vertical);
            } else {
                switch (contentPolicy) {
                    case "minFit":
                        panel.AddContentSizeFitter(ContentSizeFitter.FitMode.MinSize);
                        break;
                    case "preferredFit":
                        panel.AddContentSizeFitter(ContentSizeFitter.FitMode.PreferredSize, dir);
                        break;
                    default:
                        break;
                }
            }


            if (reader.GetAttribute("padding") != null ) {
                int[] padding = XmlUtilities.ToIntArray(reader.GetAttribute("padding"));
                panel.SetPadding(padding);
            }

            if( reader.GetAttribute("background") != null ) {
                if (reader.GetAttribute("background") == "none") {
                    GameObject.Destroy(panel.background);
                }
            }

            while (reader.Read()) {
                if (reader.NodeType == XmlNodeType.Element) {
                    XmlReader subReader = reader.ReadSubtree();
                    GUIController.ReadElement(subReader, panel);
                    subReader.Close();
                }
            }

            return panel;
        }

        private void SetPadding(int[] padding) {
            layout.padding = new RectOffset(padding[0], padding[1], padding[2], padding[3]);
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
            foreach (IWidget child in childs) {
                child.Update(null);
            }
            base.Update(null);
        }
    }
}