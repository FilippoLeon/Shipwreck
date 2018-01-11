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

        UnityEngine.UI.LayoutElement layoutComponent;

        protected object[] args = null;
        protected Dictionary<string, int> argDict = new Dictionary<string, int>();

        private int counter = 0;

        public Action ChangeArguments { set; get; }

        public void SetParameters(object[] args) {
            Debug.Log("Change Parameters!");

            this.args = args;

            if(ChangeArguments != null) ChangeArguments();
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

        public void SetParent(IWidget parent) {
            this.parent = parent;
            if (parent != null) {
                GameObject.transform.SetParent(parent.GameObject.transform);

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
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }
        public static T GetPropValue<T>(object obj, String name) {
            object retval = GetPropValue(obj, name);
            if (retval == null) { return default(T); }

            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }
    }
}