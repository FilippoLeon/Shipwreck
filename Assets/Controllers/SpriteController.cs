using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour {
    public static SpriteLoader spriteLoader;
    
	void Start () {
        if (spriteLoader != null) {
            Debug.LogError("Only one instance of 'SpriteController' is allowed.");
        } else {
            spriteLoader = new SpriteLoader("Sprites");
        }
	}
	
}
