using LUA;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GUI {
    [MoonSharpUserData]
    abstract public class Widget : Emitter<Widget>, IWidget {
        /// <summary>
        /// 
        /// </summary>
        private IWidget parent = null;

        private IWidget root = null;
        public IWidget Root {
            get { return root; }
            set { root = value; }
        }

        /// <summary>
        /// Returns the parameter with key "name" stored in this Widget. Currently parameters should be stored in a Root widget.
        /// </summary>
        /// <param name="name">The name of the parameter to fetch.</param>
        /// <returns>The value of the fetched parameter, upcasted to object.</returns>
        public override object GetParameter(string name) {
            return args[argDict[name]];
        }
        public override void SetParameter(string name, object val) {
            throw new NotImplementedException();
        }
        
        public virtual void Dispose() {
            GameObject.Destroy(GameObject);
        }

        private string tooltipText;
        public string GetToolTipText() {
            if (tooltipText != null) return tooltipText;
            return id;
        }

        public string Tooltip {
            set {
                tooltipText = value;
            }
        }

        protected UnityEngine.UI.LayoutElement layoutComponent;

        protected object[] args = null;
        protected Dictionary<string, int> argDict = new Dictionary<string, int>();

        private int counter = 0;

        public Action ChangeArguments { set; get; }

        protected Dictionary<int, GenericAction> addedAction = new Dictionary<int, GenericAction>();

        protected SortedDictionary<int, object> values = new SortedDictionary<int, object >();

        public virtual void SetValue(object value, int index) {
            values[index] = value;
        }

        public void SetParameters(object[] args) {
            //Debug.Log("Change Parameters!");

            this.args = args;

            if (ChangeArguments != null) ChangeArguments();

            if (this is IWidgetContainer) {
                Emit("OnArgumentChange", new object[] { GUIController.childs, Verse.Instance, this });
            }
        }

        public void AddParameter(string name) {
            argDict[name] = counter++;
        }

        string id;
        override public string Id {
            set {
                id = value;
                GameObject.name = id;
            }
            get {
                return id;
            }
        }

        public override string Category {
            get {
                return "UI";
            }
        }

        static int staticId;

        public Widget(bool createNew, string id = null) {
            if (id == null) {
                this.id = "W" + Convert.ToString(staticId++);
            }
        }

        public Widget(string id = null) {
            GameObject = new UnityEngine.GameObject();
            GameObject.AddComponent<WidgetComponent>().widget = this;
            if(id == null) {
                Id = "W" + Convert.ToString(staticId++);
            } else {
                Id = id;
            }
            //EventTrigger.Entry entry = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
            //entry.callback.AddListener( (BaseEventData d) => {
            //    Debug.Log("Enter!");
            //    GUIController.HoverObject = this;
            //} );
            //GameObject.AddComponent<EventTrigger>().triggers.Add(entry);

            GameObject.transform.SetParent(GUIController.Canvas.transform);

            layoutComponent = GameObject.AddComponent<UnityEngine.UI.LayoutElement>();
        }

        public virtual void SetNonExpanding(int minWidth = -1) {
            if (minWidth >= 0) layoutComponent.minWidth = minWidth;
            layoutComponent.preferredWidth = 0;
        }

        public UnityEngine.GameObject GameObject { set; get; }
        


        protected void LinkArgNameToValue(string argName, string propName, int index) {
            values[index] = null;

            System.Action<object[]> action = (object[] o) => {
                IEmitter arg = Root.GetArgument(argName);
                if (GetPropValue(arg, propName) == null ) {
                    //string a = argName + "......." + propName + " .. "+index;
                    //Debug.Log(argName);
                    //Debug.Log(propName);
                    return;
                }
                //object val = GetPropValue(arg, propName).ToString();
                //Debug.Log(String.Format("Argument object {0} has value {1}", propName, val.ToString()));
                SetValue(GetPropValue(arg, propName), index);
            };
            Root.ChangeArguments += () => {
                // Register the "Value update"-Lambda with the Root element's argument with this name.
                // Deregister old lambda from the element.
                // TODO: if argument changes, we should deregister the addedAction.
                if ( addedAction.ContainsKey(index) ) {
                    Root.GetArgument(argName).RemoveAction("On" + propName + "Changed", addedAction[index]);
                }
                addedAction[index] = Root.GetArgument(argName).AddAction("On" + propName + "Changed", action);
            };

        }

        public void SetParent(IWidget parent) {
            this.parent = parent;
            if (parent != null) {
                //Debug.LogWarningFormat("Moving {0} to {1}...", GameObject.name, parent.GetContentGameObject().name);
                GameObject.transform.SetParent(parent.GetContentGameObject().transform);

                if (parent.Root == null) {
                    Root = parent;
                } else {
                    Root = parent.Root;
                }
            }
        }

        private bool hidden = false;
        public void Toggle() {
            if (hidden) Show(); else Hide();
        }

        public Widget AddTo(WidgetContainer container) {
            container.AddChild(this);
            return this;
        }

        public void Show() {
            GameObject.SetActive(true);
            Emit("OnShow", new object[] { GUIController.childs, Verse.Instance, this });
            hidden = false;
        }

        public void Hide() {
            GameObject.SetActive(false);
            Emit("OnHide", new object[] { GUIController.childs, Verse.Instance, this });
            hidden = true;
        }

        public virtual void Update(object[] args) {
            Emit("OnUpdate", args);
        }

        internal virtual void ReadCurrentElement(XmlReader reader, IWidget parent) {
            base.ReadCurrentElement(reader);

            if (reader.GetAttribute("id") != null) {
                Id = reader.GetAttribute("id");
            }

            string preferredSize = reader.GetAttribute("preferredSize");
            if (preferredSize != null) {
                UnityEngine.Vector2 pfs = XmlUtilities.ToVector2(preferredSize);
                SetPreferredSize((int) pfs.x, (int) pfs.y);
            }
            string minSize = reader.GetAttribute("minSize");
            if (minSize != null) {
                UnityEngine.Vector2 pfs = XmlUtilities.ToVector2(minSize);
                SetMinSize((int) pfs.x, (int) pfs.y);
            }

            SetParent(parent);
        }

        protected int valueIndex = 0;

        public void FinalizeRead() {

            Root.ChangeArguments += () => {
                foreach (GenericAction act in addedAction.Values) {
                    act.Call(this, new object[] { });
                }
            };
        }

        protected void ReadSubElement(XmlReader reader) {
            //GUIController.ReadElement(subReader, progressBar);
            
            switch (reader.Name) {
                case "Value":
                    string argName = "@" + reader.GetAttribute("argument");
                    string propName = reader.ReadElementContentAsString();
                    int idx = valueIndex++;
                    LinkArgNameToValue(argName, propName, idx);
                    break;
                case "Tooltip":
                    tooltipText = reader.ReadElementContentAsString();
                    break;
                default:
                    base.ReadElement(reader);
                    break;
            }
        }
        
        public void SetPreferredSize(int w, int h = -1) {
            if( w >= 0) layoutComponent.preferredWidth = w;
            if (h >= 0) layoutComponent.preferredHeight = h;
        }
        public Widget SetMinSize(int w, int h = -1) {
            if (w >= 0) layoutComponent.minWidth = w;
            if (h >= 0) layoutComponent.minHeight = h;
            return this;
        }

        public Dictionary<string, int> GetArgDict() {
            return argDict;
        }
        public object[] GetArgs() {
            return args;
        }

        public IEmitter GetArgument(string name) {
            if (argDict.ContainsKey(name)) {
                int num = argDict[name];
                if (args != null && args.Length > num) {
                    return args[num] as IEmitter;
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }
        public static object GetPropValue(object obj, String name) {
            foreach (String part in name.Split('.')) {
                if (obj == null) { return null; }

                Type type = obj.GetType();

                object param = null;
                if( obj is IEmitter ) {
                    param =  (obj as IEmitter).GetParameter(name);
                }
                if (param != null) {
                    obj = param;
                } else {
                    PropertyInfo info = type.GetProperty(part);
                    if (info == null) { return null; }

                    obj = info.GetValue(obj, null);
                }
            }
            return obj;
        }
        public static T GetPropValue<T>(object obj, String name) {
            object retval = GetPropValue(obj, name);
            if (retval == null) { return default(T); }

            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }

        public virtual GameObject GetContentGameObject() {
            return GameObject;
        }
    }
}