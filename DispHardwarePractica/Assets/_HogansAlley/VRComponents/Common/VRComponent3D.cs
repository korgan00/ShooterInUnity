using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VRComponent3D : VRComponent {


    // Use this for initialization
    public override void Start() {
        base.Start();
        _cameraRaycaster.cast3D = true;
    }
}
