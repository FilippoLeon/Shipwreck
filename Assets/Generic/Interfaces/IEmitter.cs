﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IEmitter {
    GenericAction AddAction(string eventName, System.Action<object[]> act);
    void RemoveAction(string eventName, GenericAction act);
    string Category { get; }
    string Id { set; get; }

    object GetParameter(string name);
    object SetParameter(string name, object o);
}

/// <summary>
/// Direction the part is facing.
/// </summary>
public enum Direction {
    Down, Right, Up, Left
};

public interface IEmitter<T> : IEmitter where T: class {

    void register(IObserver<T> observer);

    void deregister(IObserver<T> observer);

    void Emit(String signal, object[] args);
}
