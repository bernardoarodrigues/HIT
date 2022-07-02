using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour {
	public TextMeshProUGUI score;

    public TextMeshProUGUI coins;

	public static GameOverUI instance;
	
	private void Awake() {
		instance = this;
	}

	public void UpdateScore() {
		score.text = "Score: " + ScoreController.instance.score;
        int earnedMoney = (int) ((ScoreController.instance.score / 10) * Random.Range(1f, 3f));
		if(earnedMoney < 0) earnedMoney = 0;
        coins.text = "Moedas: " + earnedMoney;
        int currentMoney = PlayerPrefs.GetInt("money", 0);
        PlayerPrefs.SetInt("money", currentMoney + earnedMoney);
	}

}
