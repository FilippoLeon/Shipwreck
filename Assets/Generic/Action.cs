using MoonSharp.Interpreter;
using System;
using System.ComponentModel;
using System.Xml.Serialization;
using UnityEngine;

public class GenericAction {
    ActionType type;
    string name;
    string content;
    Closure closure;
    System.Action<object[]> sysAction;

    public enum ActionType { FunctionName, Inline, None, Closure, System };

    public GenericAction(IEmitter emitter, string eventName, string type, string content) {
        this.content = content;
        if (content == "") {
            Debug.LogWarning("Empty or Invalid Action field.");
            this.type = ActionType.None;
            return;
        }

        switch (type) {
            case "script":
                this.type = ActionType.FunctionName;
                break;
            default:
                this.type = ActionType.Inline;
                name = emitter.Id + "_" + eventName;

                LUA.ScriptLoader.DoString(emitter.Category, name + " = " + content);
                return;
        }
    }

    public GenericAction(Closure closure) {
        this.closure = closure;
        this.type = ActionType.Closure;
    }


    public GenericAction(System.Action<object[]> act) {
        this.sysAction = act;
        this.type = ActionType.System;
    }

    public DynValue Call(IEmitter emitter, object[] args) {
        switch (type) {
            case ActionType.FunctionName:
                return LUA.ScriptLoader.Call(emitter.Category, content, args);
            case ActionType.Inline:
                return LUA.ScriptLoader.Call(emitter.Category, name, args);
            case ActionType.Closure:
                try {
                    return closure.Call(args);
                } catch (ScriptRuntimeException e) {
                    Debug.LogError("Script exception: " + e.DecoratedMessage);
                    return null;
                } catch (ArgumentException e) {
                    Debug.LogError("Script exception while running call to action: " + e.Message);
                    return null;
                }
            case ActionType.System:
                sysAction(args);
                return null;
            default:
                return null;
        }
    }
}
