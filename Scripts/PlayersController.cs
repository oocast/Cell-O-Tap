using UnityEngine;
using System.Collections;

public class PlayersController : MonoBehaviour {
	PlayerController[] players;
	public static int[] scores;

	// Use this for initialization
	void Start () {
		scores = new int[4];
		players = new PlayerController[4];
		for(int i = 0; i < 4; i++) {
			players[i] = transform.GetChild(i).GetComponent<PlayerController>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.I)) {
			DispalyScores();
		}
		for (int i = 0; i < 4; i++) {
			scores[i] = players[i].GetScore();
		}
	}

	public void AddScore(int score, int left, int right) {
		players[left].AddScore(score);
		players[right].AddScore(score);
	}

	public void AddScore(int score, int index) {
		players[index].AddScore(score);
	}

	public void DispalyScores() {
		for(int i = 0; i < 4; i++) {
			Debug.Log ("Player " + i + players[i].GetScoreInfo());
		}
	}
}
