using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHandler : MonoBehaviour {

    [SerializeField] private float _impulseMagnitude = 10f;
    [SerializeField] private AudioClip _fireSound;
    [SerializeField] private AudioClip _hitSound;
    [SerializeField] private ParticleSystem _particleSystem;

    private Rigidbody _rigBody;
    private AudioSource _audioSource;



    // Use this for initialization
    void Start () {
        if (_particleSystem == null) {
            Debug.LogWarning("No se encuentra el ParticleSystem del Bullet");
        }
        _rigBody = GetComponent<Rigidbody>();
        if (_rigBody == null) {
            Debug.LogWarning("No se encuentra el Rigidbody del Bullet");
        }
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) {
            Debug.LogWarning("No se encuentra el AudioSource del Bullet");
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Shoot(Vector3 direction) {
        Start();
        _audioSource.PlayOneShot(_fireSound);
        _rigBody.AddForce(direction * _impulseMagnitude, ForceMode.Impulse);
    }

    public void OnCollisionEnter(Collision collision) {
        _audioSource.PlayOneShot(_hitSound);
        if (collision.gameObject.name.ToLower() != "ground") { 
            _particleSystem.Play();
        }
    }
}
