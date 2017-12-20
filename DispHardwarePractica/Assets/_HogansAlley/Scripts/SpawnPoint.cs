using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

	// Use this for initialization
	void Awake() {
        PeopleSpawner.instance.SubscribeSpawnPoint(this);
    }
	
	// Update is called once per frame
	void Update() {
		
	}
}
