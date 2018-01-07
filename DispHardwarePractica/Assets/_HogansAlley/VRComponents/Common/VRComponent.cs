using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VRComponent : MonoBehaviour {


    protected CameraRaycaster _cameraRaycaster;

    // Use this for initialization
    public virtual void Start() {
        _cameraRaycaster = CameraRaycaster.instance;
    }

    public virtual void Leave() { }

    public virtual void Hover() { }
}
