using UnityEngine;
using System.Collections;

public class BombGenerator : MonoBehaviour {
	public GameObject bombPrefab;
	int bombPhase;
	bool genBombForth = false;

	// Use this for initialization
	void Start () {
		GameController.onReversePhaseStart += GenerateBombWrapper;
		GameController.onReversePhaseStart += ClearBomb;
		GameController.onGameOver += ClearBomb;
	}
	
	// Update is called once per frame
	void Update () {
		if (!genBombForth && GameController.difficulty == 3) {
			genBombForth = true;
			GenerateBombWrapper();
		}
	
	}

	void GenerateBombWrapper() {
		bombPhase = GameController.difficulty;
		Invoke("GenerateBomb", Random.Range(12f, 20f));
	}

	void GenerateBomb() {
		if (GameController.difficulty == bombPhase) {
			var bomb = Instantiate(bombPrefab).GetComponent<BombController>();
			bomb.transform.parent = transform;
			bomb.Initialize(Random.Range(-20f, 20f),
			                MovingPattern.SPIRAL,
			                Random.Range(0,4),
			                2f,
			                1f,
			                100f,
			                0,
			                0
			                );
		}
	}

	void ClearBomb() {
		foreach(Transform child in transform) {
			child.GetComponent<BombController>().OnDie();
		}
	}
}
