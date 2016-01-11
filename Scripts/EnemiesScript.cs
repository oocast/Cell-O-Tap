using UnityEngine;
using System.Collections;

public class EnemiesScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameController.onReversePhaseStart += ClearStage;
		GameController.onGameOver += ClearStage;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void ClearStage() {
		foreach (Transform child in transform) {
			child.GetComponent<Enemy>().OnDie();
		}
	}
}
