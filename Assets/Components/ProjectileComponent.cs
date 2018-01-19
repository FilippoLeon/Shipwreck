using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileComponent : EntityComponent<ConcreteEntity> {

    void Start() {
    }

    void Update() {
        //RaycastHit hitInfo;
        //Ray ray = new Ray(gameObject.transform.position - 0.1f*Vector3.forward, Vector2.zero);
        //Debug.DrawRay(gameObject.transform.position, Vector3.forward);
        RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Vector2.zero);
        if ( hit.transform != null ) {
            Debug.Log(hit.transform.gameObject.name);
            ObserverBehaviour<Part> emitterObserver = hit.transform.gameObject.GetComponent<ObserverBehaviour<Part>>();
            if (emitterObserver != null) {
                (Emitter as Projectile).Hit(emitterObserver.Emitter);
            }
        }
    }
}
