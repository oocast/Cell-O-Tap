using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreUI : MonoBehaviour {
	Text text;
	public int playerNo;
	bool scoring;
	public float movespeed = 2;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
		scoring = false;
		GameController.onGameStart += BeginScore;
		GameController.onGameOver += Ending;
	}
	
	// Update is called once per frame
	void Update () {
		if (scoring) {
			text.text = PlayersController.scores [playerNo].ToString ();
		} else {
			text.text = "";
		}

	}

	void Ending() {
		text.color = Color.white;
		StartCoroutine (MovingToCenter());
	}

	IEnumerator MovingToCenter() {
		for (int i = 0; i < 100; i++) {
			transform.Translate (transform.up * Time.fixedDeltaTime * movespeed, Space.World);
			yield return new WaitForFixedUpdate();
		}
	}

	void BeginScore() {
		scoring = true;
	}
}
