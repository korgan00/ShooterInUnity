using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRGameObject : VRComponent {

    public UnityEvent OnClick = new UnityEvent();
    public UnityEvent OnHover = new UnityEvent();
    public UnityEvent OnLeave = new UnityEvent();

    // Use this for initialization
    public override void Start() {
        base.Start();
    }

    // Update is called once per frame
    void Update() {

    }

    [ContextMenu("Do Click")]
    public void Click() {
        OnClick.Invoke();
    }

    public override void Leave() {
        OnLeave.Invoke();
    }

    public override void Hover() {
        OnHover.Invoke();
    }
}
