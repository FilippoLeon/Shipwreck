using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetComponent : ObserverBehaviour<Planet> {
    
    public override void HandleEvent(string signal, object[] args) {
        throw new NotImplementedException();
    }

    public override void HandleEvent(string signal) {
        base.HandleEvent(signal);
    }
}
