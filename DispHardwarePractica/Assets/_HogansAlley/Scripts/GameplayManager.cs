using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour {

    [SerializeField] private float _maxTime = 60f;
    [SerializeField] private TextMesh _text;

    [SerializeField] private float _points;
    private float _playingTime = 0f;

    public static GameplayManager instance { get; private set; }

    // Use this for initialization
    void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
        _playingTime += Time.deltaTime;
        if (_playingTime > _maxTime) {
            Application.Quit();
        }
        _text.text = "Time: " + (int) (_playingTime) + "\nScore: " + _points;
    }

    public void AddPoints(float p) {
        _points += p;
    }
}
