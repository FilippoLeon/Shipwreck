using LUA;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using UnityEngine;

namespace GUI {
    [MoonSharpUserData]
    abstract public class Widget : Emitter<Widget>, IWidget {
        private IWidget parent = null, root = null;
        public IWidget Root {
            get { return root; }
            set { root = value; }
        }

        public override object GetParameter(string name) {
            return args[argDict[name]];
        }
        public override void SetParameter(string name, object val) {
            throw new NotImplementedException();
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
                Emit("OnArgumentChange", new object[] { GUIController.childs, Verse.Instance });
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

        public Widget(bool createNew) {
            id = "W" + Convert.ToString(staticId++);
        }

        public Widget() {
            GameObject = new UnityEngine.GameObject();
            Id = "W" + Convert.ToString(staticId++);

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
                GameObject.transform.SetParent(parent.GetContentGameObject().transform);

                if (parent.Root == null) {
                    Root = parent;
                } else {
                    Root = parent.Root;
                }
            }
        }

        public virtual void Update(object[] args) {
            Emit("OnUpdate", args);
        }

        internal void ReadElement(XmlReader reader, IWidget parent) {
            base.ReadCurrentElement(reader);

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
                default:
                    base.ReadElement(reader);
                    break;
            }
        }

        public void SetPreferredSize(int w, int h = -1) {
            if( w >= 0) layoutComponent.preferredWidth = w;
            if (h >= 0) layoutComponent.preferredHeight = h;
        }
        public void SetMinSize(int w, int h = -1) {
            if (w >= 0) layoutComponent.minWidth = w;
            if (h >= 0) layoutComponent.minHeight = h;
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

        public GameObject GetContentGameObject() {
            return GameObject;
        }
    }
}