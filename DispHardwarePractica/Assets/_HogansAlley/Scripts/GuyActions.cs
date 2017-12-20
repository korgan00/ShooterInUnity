using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuyActions : MonoBehaviour {

    [SerializeField] private PersonType _personType = PersonType.GOOD;
    [SerializeField] private float _minShootTime = 1f;
    [SerializeField] private float _bangShowTime = 1f;
    [SerializeField] private float _minStayTime = 3f;
    [SerializeField] private float _maxStayTime = 5f;
    [SerializeField] private float _pointsHit = 1f;
    [SerializeField] private float _pointsMiss = -1f;

    private float _waitTime = 0f;
    private float _shootTime = 0f;

    public delegate void ShootEventHandler();
    public event ShootEventHandler OnShoot;

    private Animator _animator;
    private GameObject _bang;

    // Use this for initialization
    void Start () {
        _bang = GetComponentInChildren<MeshRenderer>().gameObject;
        _animator = GetComponentInParent<Animator>();
        if(_animator == null) {
            Debug.LogWarning("No encuentra Animator en GuyActions");
        }

        _waitTime = Random.Range(_minStayTime, _maxStayTime);
        if(_personType == PersonType.BAD) { 
            _shootTime = Random.Range(_minShootTime, _waitTime);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (_personType == PersonType.BAD) {
            _shootTime -= Time.deltaTime;
            if (_shootTime < 0) {
                _bang.SetActive(true);
                GameplayManager.instance.AddPoints(_pointsMiss);
                Invoke("HideBang", _bangShowTime);
            }
        }

        _waitTime -= Time.deltaTime;
        if (_waitTime < 0) {
            _animator.SetTrigger("EndNow");
            enabled = false;
        }
    }

    private void HideBang() {
        _bang.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Bullet") {
            Kill();
        }
    }

    public void Kill() {
        _animator.SetTrigger("EndNow");
        GameplayManager.instance.AddPoints(_pointsHit);
        enabled = false;
    }
}
