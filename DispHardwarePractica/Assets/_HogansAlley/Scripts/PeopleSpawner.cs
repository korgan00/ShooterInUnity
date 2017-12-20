using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleSpawner : MonoBehaviour {

    [SerializeField] private List<SpawnPoint> _spawnPoints;
    [SerializeField] private GameObject[] _personTypes;

    [SerializeField] private float _minSpawnTime = 3f;
    [SerializeField] private float _maxSpawnTime = 5f;

    private bool _stop = false;

    public static PeopleSpawner instance { get; private set; }

    // Use this for initialization
    void Awake() {
        instance = this;
    }

    // Use this for initialization
    void Start() {
        StartCoroutine(SpawnPeople());
	}

    // Update is called once per frame
    void Update() {

    }

    private IEnumerator SpawnPeople() {
        while (!_stop) {
            yield return new WaitForSeconds(Random.Range(_minSpawnTime, _maxSpawnTime));

            GameObject type = _personTypes[Random.Range(0, _personTypes.Length)];
            Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Count)].transform;

            Instantiate(type, spawnPoint);
        }
    }

    public void Stop() {
        _stop = true;
    }

    public void SubscribeSpawnPoint(SpawnPoint point) {
        _spawnPoints.Add(point);
    }

}
