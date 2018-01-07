using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreRankings : MonoBehaviour {

    public struct ScoreData {
        public string name;
        public float score;
    }
    
    public static ScoreRankings instance { get; private set; }
    public List<ScoreData> scores { get; private set; }

    public void AddScore(ScoreData newScore) {
        scores.Add(newScore);
        scores.Sort((scr1, scr2) => scr2.score.CompareTo(scr1.score));
    }

    public ScoreData BestScore() {
        return scores[0];
    }

	// Use this for initialization
	void Awake () {
        if (instance != null && instance != this) {
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
        scores = new List<ScoreData>();
        scores.Add(new ScoreData() { score = 0, name = "" });
        DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
}
