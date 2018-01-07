using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VRComponentUI : VRComponent {


    // Use this for initialization
    public override void Start() {
        base.Start();
        _cameraRaycaster.castUI = true;
    }
    
}
