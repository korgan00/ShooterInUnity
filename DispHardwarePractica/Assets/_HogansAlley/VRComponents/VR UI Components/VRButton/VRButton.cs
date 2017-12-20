using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AddComponentMenu("VR UI/Button")]
public class VRButton : VRComponent {

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
        GetComponent<Animator>().SetBool("Hover", false);
    }
    
    public override void Leave() {
        OnLeave.Invoke();
        GetComponent<Animator>().SetBool("Hover", false);
    }
    
    public override void Hover() {
        OnHover.Invoke();
        GetComponent<Animator>().SetBool("Hover", true);
    }
#if UNITY_EDITOR
    [MenuItem("GameObject/VR UI/Button", false, priority: 7)]
    private static void NewButtonObject() {
        VRButton newButton = (new GameObject("VR Button")).AddComponent<VRButton>();
    }
#endif
}