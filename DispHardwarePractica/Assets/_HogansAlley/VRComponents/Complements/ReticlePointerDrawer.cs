// Copyright 2015 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;

/// Draws a circular reticle in front of any object that the user points at.
/// The circle dilates if the object is clickable.
[AddComponentMenu("VR UI/ReticlePointer")]
[RequireComponent(typeof(Renderer))]
[ExecuteInEditMode]
public class ReticlePointerDrawer : MonoBehaviour {
    /// Number of segments making the reticle circle.
    public int reticleSegments = 20;

    /// Growth speed multiplier for the reticle/
    public float reticleGrowthSpeed = 8.0f;

    // Private members
    private Material _materialComp;

    // Current inner angle of the reticle (in degrees).
    private float _reticleInnerAngle = 0.0f;
    // Current outer angle of the reticle (in degrees).
    private float _reticleOuterAngle = 0.5f;
    // Current distance of the reticle (in meters).
    private float _reticleDistanceInMeters = 10.0f;

    public float reticleDistanceInMeters {
        get { return _reticleDistanceInMeters; }
        set { _reticleDistanceInMeters = Mathf.Clamp(value, RETICLE_DISTANCE_MIN, RETICLE_DISTANCE_MAX); }
    }

    public float reticleInnerAngle {
        get { return _reticleInnerAngle; }
        set { _reticleInnerAngle = value < RETICLE_MIN_INNER_ANGLE ? RETICLE_MIN_INNER_ANGLE : value; }
    }

    public float reticleOuterAngle {
        get { return _reticleOuterAngle; }
        set { _reticleOuterAngle = value < RETICLE_MIN_OUTER_ANGLE ? RETICLE_MIN_OUTER_ANGLE : value; }
    }
    
    // Minimum inner angle of the reticle (in degrees).
    private const float RETICLE_MIN_INNER_ANGLE = 0.0f;
    // Minimum outer angle of the reticle (in degrees).
    private const float RETICLE_MIN_OUTER_ANGLE = 0.5f;
    // Angle at which to expand the reticle when intersecting with an object
    // (in degrees).
    private const float RETICLE_GROWTH_ANGLE = 1.5f;

    // Minimum distance of the reticle (in meters).
    private const float RETICLE_DISTANCE_MIN = 0.45f;
    // Maximum distance of the reticle (in meters).
    private const float RETICLE_DISTANCE_MAX = 10.0f;

    // Current inner and outer diameters of the reticle,
    // before distance multiplication.
    private float _reticleInnerDiameter = 0.0f;
    private float _reticleOuterDiameter = 0.0f;

    private CameraRaycaster _raycaster = null;

    void Start() {
        _raycaster = CameraRaycaster.instance;
        if (!Application.isPlaying) return;

        CreateReticleVertices();
        _materialComp = gameObject.GetComponent<Renderer>().material;
    }

    void Update() {
        if (!Application.isPlaying) return;

        if (_raycaster.raycastInfo.isColliding) {
            GameObject targetObject = _raycaster.raycastInfo.hit;
            Vector3 intersectionPosition = _raycaster.raycastInfo.intersectionPoint;
            Ray intersectionRay = new Ray(_raycaster.castingCamera.transform.position, intersectionPosition);
            bool isInteractive = _raycaster.raycastInfo.isCollidingVRComponent;
            OnPointerHover(targetObject, intersectionPosition, intersectionRay, isInteractive);
        } else {
            OnPointerExit(null);
        }

        UpdateDiameters();
    }

    /// This is called when the 'BaseInputModule' system should be enabled.
    public void OnInputModuleEnabled() { }

    /// This is called when the 'BaseInputModule' system should be disabled.
    public void OnInputModuleDisabled() { }

    /// Called when the user is pointing at valid GameObject. This can be a 3D
    /// or UI element.
    ///
    /// The targetObject is the object the user is pointing at.
    /// The intersectionPosition is where the ray intersected with the targetObject.
    /// The intersectionRay is the ray that was cast to determine the intersection.
    public void OnPointerEnter(GameObject targetObject, Vector3 intersectionPosition, Ray intersectionRay, bool isInteractive) {
        SetPointerTarget(intersectionPosition, isInteractive);
    }

    /// Called every frame the user is still pointing at a valid GameObject. This
    /// can be a 3D or UI element.
    ///
    /// The targetObject is the object the user is pointing at.
    /// The intersectionPosition is where the ray intersected with the targetObject.
    /// The intersectionRay is the ray that was cast to determine the intersection.
    public void OnPointerHover(GameObject targetObject, Vector3 intersectionPosition, Ray intersectionRay, bool isInteractive) {
        SetPointerTarget(intersectionPosition, isInteractive);
    }

    /// Called when the user's look no longer intersects an object previously
    /// intersected with a ray projected from the camera.
    /// This is also called just before **OnInputModuleDisabled** and may have have any of
    /// the values set as **null**.
    public void OnPointerExit(GameObject targetObject) {
        reticleDistanceInMeters = RETICLE_DISTANCE_MAX;
        reticleInnerAngle = RETICLE_MIN_INNER_ANGLE;
        reticleOuterAngle = RETICLE_MIN_OUTER_ANGLE;
    }

    /// Called when a trigger event is initiated. This is practically when
    /// the user begins pressing the trigger.
    public void OnPointerClickDown() { }

    /// Called when a trigger event is finished. This is practically when
    /// the user releases the trigger.
    public void OnPointerClickUp() { }

    public float GetMaxPointerDistance() {
        return RETICLE_DISTANCE_MAX;
    }

    public void GetPointerRadius(out float enterRadius, out float exitRadius) {
        float min_inner_angle_radians = Mathf.Deg2Rad * RETICLE_MIN_INNER_ANGLE;
        float max_inner_angle_radians = Mathf.Deg2Rad * (RETICLE_MIN_INNER_ANGLE + RETICLE_GROWTH_ANGLE);

        enterRadius = 2.0f * Mathf.Tan(min_inner_angle_radians);
        exitRadius = 2.0f * Mathf.Tan(max_inner_angle_radians);
    }

    private void CreateReticleVertices() {
        Mesh mesh = new Mesh();
        gameObject.AddComponent<MeshFilter>();
        GetComponent<MeshFilter>().mesh = mesh;

        int segments_count = reticleSegments;
        int vertex_count = (segments_count + 1) * 2;

        #region Vertices

        Vector3[] vertices = new Vector3[vertex_count];

        const float kTwoPi = Mathf.PI * 2.0f;
        int vi = 0;
        for (int si = 0; si <= segments_count; ++si) {
            // Add two vertices for every circle segment: one at the beginning of the
            // prism, and one at the end of the prism.
            float angle = (float) si / (float) (segments_count) * kTwoPi;

            float x = Mathf.Sin(angle);
            float y = Mathf.Cos(angle);

            vertices[vi++] = new Vector3(x, y, 0.0f); // Outer vertex.
            vertices[vi++] = new Vector3(x, y, 1.0f); // Inner vertex.
        }
        #endregion

        #region Triangles
        int indices_count = (segments_count + 1) * 3 * 2;
        int[] indices = new int[indices_count];

        int vert = 0;
        int idx = 0;
        for (int si = 0; si < segments_count; ++si) {
            indices[idx++] = vert + 1;
            indices[idx++] = vert;
            indices[idx++] = vert + 2;

            indices[idx++] = vert + 1;
            indices[idx++] = vert + 2;
            indices[idx++] = vert + 3;

            vert += 2;
        }
        #endregion

        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateBounds();
    }

    private void UpdateDiameters() {
        float inner_half_angle_radians = Mathf.Deg2Rad * reticleInnerAngle * 0.5f;
        float outer_half_angle_radians = Mathf.Deg2Rad * reticleOuterAngle * 0.5f;

        float inner_diameter = 2.0f * Mathf.Tan(inner_half_angle_radians);
        float outer_diameter = 2.0f * Mathf.Tan(outer_half_angle_radians);

        _reticleInnerDiameter = Mathf.Lerp(_reticleInnerDiameter, inner_diameter, Time.deltaTime * reticleGrowthSpeed);
        _reticleOuterDiameter = Mathf.Lerp(_reticleOuterDiameter, outer_diameter, Time.deltaTime * reticleGrowthSpeed);

        _materialComp.SetFloat("_InnerDiameter", _reticleInnerDiameter * reticleDistanceInMeters);
        _materialComp.SetFloat("_OuterDiameter", _reticleOuterDiameter * reticleDistanceInMeters);
        _materialComp.SetFloat("_DistanceInMeters", reticleDistanceInMeters);
    }

    private void SetPointerTarget(Vector3 target, bool interactive) {
        Vector3 targetLocalPosition = transform.InverseTransformPoint(target);

        reticleDistanceInMeters = Mathf.Clamp(targetLocalPosition.z, RETICLE_DISTANCE_MIN, RETICLE_DISTANCE_MAX);
        if (interactive) {
            reticleInnerAngle = RETICLE_MIN_INNER_ANGLE + RETICLE_GROWTH_ANGLE;
            reticleOuterAngle = RETICLE_MIN_OUTER_ANGLE + RETICLE_GROWTH_ANGLE;
        } else {
            reticleInnerAngle = RETICLE_MIN_INNER_ANGLE;
            reticleOuterAngle = RETICLE_MIN_OUTER_ANGLE;
        }
    }
}
