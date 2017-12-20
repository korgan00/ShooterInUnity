using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CameraController : MonoBehaviour {

    [SerializeField] private float _camSens = 0.5f;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private bool _useLaserGun;
    [SerializeField] private float _maxLaserTime = 0.2f;

    private float _laserTime = 0.0f;
    private Vector3 _lastMouse = Vector3.zero;
    private LineRenderer _lineRenderer;

    // Use this for initialization
    void Start() {
        _lineRenderer = GetComponent<LineRenderer>();
        if (_lineRenderer == null) {
            Debug.LogWarning("No se encuentra el LineRenderer en el CameraController");
        } else { 
            _lineRenderer.enabled = false;
        }
    }

    // Update is called once per frame
    void Update() {
        if (!XRSettings.enabled) {
            CameraPosition();
        }
        if (Input.GetButtonDown("Fire1")) {
            if (_useLaserGun) {
                ShootALaser();
            } else {
                ShootABullet();
            }
        }
        _laserTime -= Time.deltaTime;
        if (_laserTime < 0) {
            _lineRenderer.enabled = false;
        }
    }


    void CameraPosition() {
        Vector3 mouseDelta = Input.mousePosition - _lastMouse;
        Vector3 screenDelta = new Vector3(-mouseDelta.y * _camSens, mouseDelta.x * _camSens, 0);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x + screenDelta.x, transform.eulerAngles.y + screenDelta.y, 0);
        _lastMouse = Input.mousePosition;
    }

    void ShootABullet() {
        GameObject bulletInstance = Instantiate(_bullet, transform.position, transform.rotation);
        BulletHandler handler = bulletInstance.GetComponent<BulletHandler>();
        if (handler != null) {
            handler.Shoot(transform.forward);
        } else {
            Debug.LogWarning("La Bullet no tiene Script BulletHandler");
        }
        Destroy(bulletInstance, 3f);
    }

    void ShootALaser() {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo)) {
            GuyActions guy = hitInfo.collider.GetComponent<GuyActions>();
            if (guy != null) {
                guy.Kill();
                _laserTime = _maxLaserTime;
                _lineRenderer.enabled = true;
                _lineRenderer.SetPosition(1, hitInfo.point);
            }
        }
    }
}
