using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour {

    public Text bestScoreText;

    void Start() {
        if (bestScoreText == null) {
            Debug.LogWarning("Best Score GameObject is not set");
            return;
        }
        bestScoreText.text = "Best Score: " + ScoreRankings.instance.BestScore().score;
    }

    public void PlayGame() {
        Debug.Log("GAME LOAD");
        SceneManager.LoadScene("StageOne");
    }

    public void GoRankings() {
        Debug.Log("RANKINGS LOAD");
    }
}
