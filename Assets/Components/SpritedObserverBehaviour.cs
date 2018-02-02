using System.Collections.Generic;
using UnityEngine;

public class SpritedObserverBehaviour<T> : ObserverBehaviour<T> where T: Entity<T> {
    List<SpriteRenderer> layers = new List<SpriteRenderer>();

    public override void HandleEvent(string signal, object[] args) {
        throw new System.NotImplementedException();
    }

    protected void CreateGraphics() {
        if(Emitter.spriteInfo == null) {
            return;
        }
        EnsureSize();
        for (int i = 0; i < Emitter.SpriteInfo.Count; ++i) {
            SpriteController.spriteLoader.LoadIntoSpriteRenderer(layers[i], Emitter.SpriteInfo.Get(i), Emitter);
        }
    }

    private void EnsureSize() {
        int oldCap = layers.Count;
        layers.Capacity = Mathf.Max(oldCap, Emitter.SpriteInfo.Count);
        for (int i = oldCap; i < Emitter.SpriteInfo.Count; ++i) {
            GameObject layer = new GameObject("layer" + i);
            layer.transform.SetParent(transform, false);
            //layer.transform.localPosition = Vector3.zero;
            layers.Add(layer.AddComponent<SpriteRenderer>());
        }
    }

    protected void UpdateGraphics() {
        EnsureSize();
        for (int i = 0; i < Emitter.SpriteInfo.Count; ++i) {
            SpriteController.spriteLoader.LoadIntoSpriteRenderer(layers[i], Emitter.SpriteInfo.Get(i), Emitter);
        }
    }
}