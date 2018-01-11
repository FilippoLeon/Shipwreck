using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class ObserverBehaviour<T> : MonoBehaviour, IObserver<T> where T: class {
    public T Emitter { set; get; }

    public abstract void HandleEvent(string signal, object[] args);
    public abstract void HandleEvent(string signal);
}
