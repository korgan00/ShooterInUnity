using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VRComponent : MonoBehaviour {


    // Use this for initialization
    public virtual void Start() {
        CameraRaycaster.AutoInstance();
    }

    public virtual void Leave() { }

    public virtual void Hover() { }
}
