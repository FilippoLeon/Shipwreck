using System.Collections.Generic;
using UnityEngine;

public class SpritedObserverBehaviour<T> : ObserverBehaviour<T> where T: Entity<T> {
    List<SpriteRenderer> layers = new List<SpriteRenderer>();

    public override void HandleEvent(string signal, object[] args) {
        throw new System.NotImplementedException();
    }

    protected void CreateGraphics() {
        if(Emitter.icon == null) {
            return;
        }
        EnsureSize();
        for (int i = 0; i < Emitter.Icon.Count; ++i) {
            SpriteController.spriteLoader.LoadIntoSpriteRenderer(layers[i], Emitter.Icon.Get(i), Emitter);
        }
    }

    private void EnsureSize() {
        int oldCap = layers.Count;
        layers.Capacity = Mathf.Max(oldCap, Emitter.Icon.Count);
        for (int i = oldCap; i < Emitter.Icon.Count; ++i) {
            GameObject layer = new GameObject("layer" + i);
            layer.transform.SetParent(transform, false);
            //layer.transform.localPosition = Vector3.zero;
            layers.Add(layer.AddComponent<SpriteRenderer>());
        }
    }

    protected void UpdateGraphics() {
        EnsureSize();
        for (int i = 0; i < Emitter.Icon.Count; ++i) {
            SpriteController.spriteLoader.LoadIntoSpriteRenderer(layers[i], Emitter.Icon.Get(i), Emitter);
        }
    }
}