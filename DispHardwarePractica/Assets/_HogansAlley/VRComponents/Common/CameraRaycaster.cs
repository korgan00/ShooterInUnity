using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRaycaster : MonoBehaviour {

    private static CameraRaycaster _instance;

    private VRComponent _lastLookedComponent = null;
    List<RaycastResult> _raycastResults = new List<RaycastResult>();

    public CameraRaycaster instance {
        get {
            if (_instance == null)
                AutoInstance();
            return _instance;
        }
    }

    public bool isColliding {
        get { return _raycastResults.Count != 0; }
    }
    public bool isCollidingVRComponent {
        get { return _lastLookedComponent != null; }
    }

    public RaycastResult lastRaycastInteractive { get; private set; }
    public Vector3 lastRaycastIntersection {
        get {
            Transform camTransform = castingCamera.transform;
            return camTransform.position + camTransform.forward * lastRaycastInteractive.distance;
        }
    }
    public Camera castingCamera {
        get { return Camera.main; }
    }

    private void Awake() {
        _instance = this;
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        _raycastResults.Clear();

        VRComponent lookedComp = null;
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = castingCamera.WorldToScreenPoint(castingCamera.transform.forward * 10);
        EventSystem.current.RaycastAll(pointerData, _raycastResults);

        List<RaycastResult> raycastVRCompResults = _raycastResults.Where((r) => { return r.gameObject.GetComponent<VRComponent>() != null; }).ToList();
        lastRaycastInteractive = new RaycastResult();

        if (raycastVRCompResults.Count > 0) {
            Debug.Log("HIT!");
            lookedComp = raycastVRCompResults[0].gameObject.GetComponent<VRComponent>();
            lastRaycastInteractive = raycastVRCompResults[0];
            raycastVRCompResults.Clear();
        }

        UpdateLookedItem(lookedComp);
    }

    void OnDrawGizmos() {
        /*
        if (Debug.isDebugBuild) {
            Transform camTransform = mainCamera.transform;
            Ray r = new Ray(camTransform.position, camTransform.forward);
            RaycastHit[] raycastHits = Physics.RaycastAll(r);
            
            Gizmos.color = (raycastHits.Length > 0 && raycastHits[0].distance != 0) ? Color.green : Color.blue;
            Gizmos.DrawRay(camTransform.position, camTransform.forward * 100);
        }
        */
        if (Debug.isDebugBuild) {
            Transform camTransform = castingCamera.transform;
            Gizmos.color = isCollidingVRComponent? Color.green : isColliding ? Color.cyan : Color.blue;
            Gizmos.DrawRay(camTransform.position, camTransform.forward * 100);

            if (isCollidingVRComponent) {
                Gizmos.color = _lastLookedComponent == null ? Color.blue : Color.green;
                Gizmos.DrawSphere(lastRaycastIntersection, 0.1f);
            }
        }
    }

    private void UpdateLookedItem(VRComponent component) {
        if (_lastLookedComponent != component) {
            if (_lastLookedComponent != null) {
                _lastLookedComponent.Leave();
            }
            if (component != null) {
                component.Hover();
            }
            _lastLookedComponent = component;
        }
    }

    public static CameraRaycaster AutoInstance() {
        if (_instance == null) {
            _instance = FindObjectOfType<CameraRaycaster>();
            if (_instance == null) {
                _instance = (new GameObject("VREventSystem_CameraRaycaster")).AddComponent<CameraRaycaster>();
            }
        }
        return _instance;
    }

}
