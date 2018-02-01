using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using LUA;
using GUI;
using UnityEngine;
using UnityEngine.EventSystems;

public class GUIController : UnityEngine.MonoBehaviour {
    private static UnityEngine.Canvas canvas;

    static public Dictionary<string, IWidget> childs = new Dictionary<string, IWidget>();

    static public Color DefaultTextColor = Color.white;

    GameObject Tooltip;
    void Start() {
        canvas = FindObjectOfType(typeof(Canvas)) as Canvas;

        canvas.GetComponent<UnityEngine.UI.CanvasScaler>().referencePixelsPerUnit = 64;

        Tooltip = Instantiate(FindObjectOfType<GUIPrefabs>().Tooltip, canvas.transform);
        //Tooltip.

        ScriptLoader.LoadScript("UI", "UI/UI.lua");

        ScriptLoader.RegisterPlaceolder("UI", typeof(GUI.Button));
        ScriptLoader.RegisterPlaceolder("UI", typeof(GUI.Panel));
        ScriptLoader.RegisterPlaceolder("UI", typeof(GUI.Label));
        ScriptLoader.RegisterPlaceolder("UI", typeof(GUI.Image));

        ScriptLoader.RegisterPlaceolder("UI", typeof(UnityEngine.Color));
        ScriptLoader.RegisterPlaceolder("UI", typeof(SpriteInfo));
        ScriptLoader.RegisterPlaceolder("UI", typeof(UI.Input));

        ScriptLoader.RegisterPlaceolder("UI", typeof(Verse));
        ScriptLoader.RegisterPlaceolder("UI", typeof(Coordinate));
        ScriptLoader.RegisterPlaceolder("UI", typeof(Projectile));

        BuildUI();
    }

    static public IWidget Find(string childName) {
        if (childs.ContainsKey(childName)) {
            return childs[childName];
        } else {
            return null;
        }
    }

    void BuildUI() {
        XmlReaderSettings settings = new XmlReaderSettings();
        XmlReader reader = XmlReader.Create(PathUtilities.GetPath("Data/UI", "UI.xml"), settings);

        reader.MoveToContent();
        UnityEngine.Debug.Assert(reader.Name == "Canvas");

        while (reader.Read()) {
            XmlNodeType nodeType = reader.NodeType;
            switch (nodeType) {
                case XmlNodeType.Element:
                    XmlReader subTree = reader.ReadSubtree();
                    ReadElement(subTree);
                    subTree.Close();
                    break;
            }
        }

        Tooltip.transform.SetAsLastSibling();
        Tooltip.GetComponent<UnityEngine.UI.Image>().sprite = SpriteController.spriteLoader.tryLoadSprite("UI", "panel_background").sprite;
    }

    public static IWidget ParseXml(XmlReader reader, Widget parent = null) {
        reader.Read();
        IWidget child = null;
        switch (reader.Name) {
            case "Panel":
                child = Panel.Create(reader, parent);
                break;
            case "ScrollView":
                child = ScrollView.Create(reader, parent);
                break;
            case "Label":
                child = Label.Create(reader, parent);
                break;
            case "Button":
                child = Button.Create(reader, parent);
                break;
            case "ProgressBar":
                child = ProgressBar.Create(reader, parent);
                break;
            case "Argument":
                reader.Read();
                parent.AddParameter(reader.ReadContentAsString());
                break;
            default:
                parent.ReadElement(reader);
                break;
        }
        if (child == null) return null;
        if( parent != null) {
            if (parent.Root == null) {
                child.Root = parent;
            } else {
                child.Root = parent.Root;
            }
            if(parent is IWidgetContainer) {
                (parent as IWidgetContainer).AddChild(child);
            }
        }


        return child;
    }

    public static void ReadElement(XmlReader reader, Widget parent = null) {
        IWidget child = ParseXml(reader, parent);

        if (child != null) {
            childs.Add(child.Id, child);

            if (child is IEmitter<Widget>) {
                (child as IEmitter<Widget>).Emit("OnCreate", new object[] { childs, Verse.Instance });
            }
        }

    }

    public static Canvas Canvas {
        get { return canvas; }
    }

    public object LUA { get; private set; }
    public static IWidget HoverObject { get; internal set; }
    

    private void Update() {
        foreach (IWidget child in childs.Values) {
            child.Update(null);
        }
        
        PointerEventData pointerData = new PointerEventData(EventSystem.current);

        pointerData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        if (results.Count > 0) {
            //foreach(RaycastResult r in results) {
            for (int i = 0; i < results.Count; ++i) {
                WidgetComponent wic = results[i].gameObject.GetComponent<WidgetComponent>();
                if (wic != null) {
                    if(wic.widget is Widget) {
                        (wic.widget as Widget).Emit("OnHover", new object[] { childs, Verse.Instance, wic.widget });
                    }
                    if( HoverObject is Widget && wic.widget != HoverObject) {
                        (HoverObject as Widget).Emit("OnHoverExit", new object[] { childs, Verse.Instance, HoverObject });
                    } 
                    HoverObject = wic.widget;
                    break;
                }
            }
            //Debug.Log(results[0].gameObject.name);
            //}
        } else {
            HoverObject = null;
        }
        //EventSystem.current.RaycastAll(Input.mousePosition, Camera.main);
        if ( HoverObject != null) {
            string ttt = HoverObject.GetToolTipText();
            if( ttt == null ) {
                Tooltip.SetActive(false);
            } else {
                Tooltip.transform.position = Input.mousePosition + new Vector3(17f,-17f, -1f);
                Tooltip.SetActive(true);
                Tooltip.GetComponentInChildren<UnityEngine.UI.Text>().text = ttt;
            }
        } else {
            Tooltip.SetActive(false);
        }
    }
}
