using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

[MoonSharpUserData]
abstract public class Emitter<T> : IXmlSerializable, IEmitter<T> where T : class {
    List<IObserver<T>> observers = new List<IObserver<T>>();

    protected Dictionary<string, List<GenericAction>> actions = new Dictionary<string, List<GenericAction>>();

    abstract public string Category { get; }
    abstract public string Id { set; get; }

    public void register(IObserver<T> observer) {
        observer.Emitter = this as T;
        observers.Add(observer);
    }


    public abstract bool HasParameter(string name);
    public abstract object GetParameter(string name);
    public abstract object SetParameter(string name, object v);

    public void deregister(IObserver<T> observer) {
        observer.Emitter = null;
        observers.Remove(observer);
    }

    virtual public void ReadXml(XmlReader reader) {
        throw new NotImplementedException();
    }

    virtual public void WriteXml(XmlWriter writer) {
        throw new NotImplementedException();
    }

    public void Emit(String signal, object[] args) {
        foreach (IObserver<T> observer in observers) {
            observer.HandleEvent(signal, args);
        }

        Call(signal, args);
    }
    public void Emit(String signal) {
        foreach (IObserver<T> observer in observers) {
            observer.HandleEvent(signal);
        }

        Call(signal, new object[] { this });
    }

    internal object Call(string eventName, object[] args) {
        DynValue ret = null;

        if (actions.ContainsKey(eventName)) {
            foreach (GenericAction act in actions[eventName]) {
                ret = act.Call(this, args);
            }
        }

        return ret;
    }

    virtual public XmlSchema GetSchema() {
        return null;
    }

    public virtual void ReadCurrentElement(XmlReader reader) {
    
        if (reader.GetAttribute("id") != null) {
            Id = reader.GetAttribute("id");
        }
    }

    public virtual void ReadElement(XmlReader reader) {
        switch (reader.Name) {
            case "Action":
                string actionName = reader.GetAttribute("event");
                string type = reader.GetAttribute("type");
                string actionFunction = reader.ReadElementContentAsString();
                AddAction(actionName, type, actionFunction);
                break;
        }
    }

    public void EnsureEventExists(string name) {
        if( !actions.ContainsKey(name)) {
            actions[name] = new List<GenericAction>();
        }
    }

    public GenericAction AddAction(string eventName, string type, string content) {
        if (eventName == null) {
            Debug.LogWarning(String.Format("Try to add empty Action with no-name to list, content ={0}.", content));
            return null;
        }
        GenericAction action = new GenericAction(this, eventName, type, content);
        EnsureEventExists(eventName);

        OnActionAdded(eventName, action);
        actions[eventName].Add(action);
        return action;
    }

    public virtual GenericAction AddAction(string eventName, System.Action<object[]> act) {
        GenericAction action = new GenericAction(act);
        EnsureEventExists(eventName);

        OnActionAdded(eventName, action);
        actions[eventName].Add(action);
        return action;
    }

    public virtual GenericAction OnActionAdded( string eventName, GenericAction action) {
        return null;
    
    }

    public virtual GenericAction AddAction(string eventName, MoonSharp.Interpreter.Closure closure) {
        GenericAction action = new GenericAction(closure);
        EnsureEventExists(eventName);
        OnActionAdded(eventName, action);
        actions[eventName].Add(action);
        return action;
    }

    public virtual void RemoveAction(string eventName, GenericAction act) {
        if (actions.ContainsKey(eventName)) {
            actions[eventName].Remove(act);
        }
    }

    public virtual void SelfDestroy() {
        Emit("OnSelfDestroy");
        observers = null;
        actions = null;
    }
}

class Reflector {
    public static object GetPropValue(object obj, String name) {
        foreach (String part in name.Split('.')) {
            if (obj == null) { return null; }

            Type type = obj.GetType();

            object param = null;
            if (obj is IEmitter) {
                param = (obj as IEmitter).GetParameter(name);
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
}