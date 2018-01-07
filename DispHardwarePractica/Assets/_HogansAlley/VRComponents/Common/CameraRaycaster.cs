using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRaycaster : MonoBehaviour {

    public class CameraRaycastInfo {
        public GameObject hit;
        public VRComponent vrComponent;
        public Vector3 intersectionPoint;
        public float intersectionDistance;
        public bool isColliding;
        public bool isCollidingVRComponent;
        
        public CameraRaycastInfo() {
            hit = null;
            vrComponent = null;
            intersectionPoint = Vector3.zero;
            intersectionDistance = 0;
            isColliding = false;
            isCollidingVRComponent = false;
        }
    }


    [SerializeField]
    private bool _castUI = true;
    [SerializeField]
    private bool _cast3D = false;
    [SerializeField]
    private float _maxDistance = 20.0f;

    private static CameraRaycaster _instance;

    public CameraRaycastInfo raycastInfoUI { get; private set; }
    public CameraRaycastInfo raycastInfo3D { get; private set; }
    public CameraRaycastInfo raycastInfo { get; private set; }
    
    public Camera castingCamera {
        get { return Camera.main; }
    }

    public static CameraRaycaster instance {
        get {
            AutoInstance();
            return _instance;
        }
        set {
            _instance = value;
        }
    }

    public bool cast3D {
        get { return _cast3D; }
        set { _cast3D = value; }
    }
    public bool castUI {
        get { return _castUI; }
        set { _castUI = value; }
    }

    private void Awake() {
        instance = this;
    }

    // Use this for initialization
    void Start() {
        raycastInfo = new CameraRaycastInfo();
        raycastInfoUI = new CameraRaycastInfo();
        raycastInfo3D = new CameraRaycastInfo();
    }

    // Update is called once per frame
    void Update() {
        if (Application.isPlaying) {
            if (castUI) {
                RaycastUI();
            }
            if (cast3D) {
                Raycast3D();
            }

            UpdateRaycastInfo();
        }
    }

    private void RaycastUI() {
        PointerEventData pointerData = new PointerEventData(EventSystem.current) {
            position = PseudoPointerPosition()
        };
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);
        
        List<RaycastResult> raycastVRCompResults = raycastResults.Where((r) => r.gameObject.GetComponent<VRComponent>() != null ).ToList();


        raycastInfoUI = new CameraRaycastInfo {
            isColliding = raycastResults.Count > 0,
            isCollidingVRComponent = raycastVRCompResults.Count > 0
        };
        RaycastResult closestRaycast;
        if (raycastInfoUI.isColliding) {
            if (raycastInfoUI.isCollidingVRComponent) {
                closestRaycast = raycastVRCompResults.OrderBy((r) => r.distance).First();
                raycastInfoUI.vrComponent = closestRaycast.gameObject.GetComponent<VRComponent>();
            } else {
                closestRaycast = raycastResults.OrderBy((r) => r.distance).First();
            }
            raycastInfoUI.hit = closestRaycast.gameObject;
            raycastInfoUI.intersectionPoint = closestRaycast.worldPosition;
            raycastInfo3D.intersectionDistance = closestRaycast.distance;
        }
        
    }

    private void Raycast3D() {
        Ray ray = new Ray(castingCamera.transform.position, castingCamera.transform.forward * 10);
        RaycastHit raycastHit;
        bool isColliding3D = Physics.Raycast(ray, out raycastHit, _maxDistance);

        raycastInfo3D = new CameraRaycastInfo {
            isColliding = isColliding3D,
            isCollidingVRComponent = false
        };
        if (isColliding3D) {
            raycastInfo3D.hit = raycastHit.collider.gameObject;
            raycastInfo3D.intersectionPoint = raycastHit.point;
            raycastInfo3D.intersectionDistance = raycastHit.distance;
            raycastInfo3D.vrComponent = raycastHit.collider.gameObject.GetComponent<VRComponent>();
            raycastInfo3D.isCollidingVRComponent = raycastInfo3D.vrComponent != null;
        }
    }

    private void UpdateRaycastInfo() {
        CameraRaycastInfo newRaycastInfo = new CameraRaycastInfo();

        if (raycastInfo3D.isCollidingVRComponent && raycastInfoUI.isCollidingVRComponent) {
            newRaycastInfo = raycastInfo3D.intersectionDistance < raycastInfoUI.intersectionDistance ? raycastInfo3D : raycastInfoUI;
        } else if (raycastInfo3D.isCollidingVRComponent) {
            newRaycastInfo = raycastInfo3D;
        } else if (raycastInfoUI.isCollidingVRComponent) {
            newRaycastInfo = raycastInfoUI;
        } else {
            if (raycastInfo3D.isColliding && raycastInfoUI.isColliding) {
                newRaycastInfo = raycastInfo3D.intersectionDistance < raycastInfoUI.intersectionDistance ? raycastInfo3D : raycastInfoUI;
            } else if (raycastInfo3D.isColliding) {
                newRaycastInfo = raycastInfo3D;
            } else if (raycastInfoUI.isColliding) {
                newRaycastInfo = raycastInfoUI;
            }
        }

        UpdateLookedItem(raycastInfo.vrComponent, newRaycastInfo.vrComponent);
        raycastInfo = newRaycastInfo;
    }

    private void UpdateLookedItem(VRComponent lastComp, VRComponent newComp) {
        if (lastComp != newComp) {
            if (lastComp != null) {
                lastComp.Leave();
            }
            if (newComp != null) {
                newComp.Hover();
            }
        }
    }

    private Vector3 PseudoPointerPosition() {
        return castingCamera.WorldToScreenPoint(castingCamera.transform.position + (castingCamera.transform.forward * 10));
    }

    private static void AutoInstance() {
        if (EventSystem.current == null && FindObjectOfType<EventSystem>() == null) {
            EventSystem evSys = (new GameObject("EventSystem")).AddComponent<EventSystem>();
            evSys.sendNavigationEvents = true;
            evSys.pixelDragThreshold = 5;
            StandaloneInputModule inputMod = evSys.gameObject.AddComponent<StandaloneInputModule>();
            inputMod.horizontalAxis = "Horizontal";
            inputMod.verticalAxis = "Vertical";
            inputMod.submitButton = "Submit";
            inputMod.cancelButton = "Cancel";
            inputMod.inputActionsPerSecond = 10;
            inputMod.repeatDelay = 0.5f;
            inputMod.forceModuleActive = false;
        }

        if (_instance == null) {
            _instance = FindObjectOfType<CameraRaycaster>();
            if (_instance == null) {
                _instance = (new GameObject("VREventSystem_CameraRaycaster")).AddComponent<CameraRaycaster>();
            }
        }
    }


    void OnDrawGizmos() {
        if (Debug.isDebugBuild && Application.isPlaying) {
            Transform camTransform = castingCamera.transform;
            Gizmos.color = raycastInfo.isCollidingVRComponent ? Color.green : raycastInfo.isColliding ? Color.cyan : Color.blue;
            Gizmos.DrawRay(camTransform.position, camTransform.forward * 100);

            if (raycastInfo.isCollidingVRComponent) {
                Gizmos.color = raycastInfo.vrComponent == null ? Color.blue : Color.green;
                Gizmos.DrawSphere(raycastInfo.intersectionPoint, 0.1f);
            }
        }
    }

}
