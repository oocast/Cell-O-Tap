using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	public delegate void ValueChange(int value);
	public static event ValueChange valueChangeEvent;
	public static float tableRadius = 37.5f;
	public static bool enemyInward = true;
	// determine the level of generated enemies
	public static int difficulty = 0;
	public static int numDeadEnemy = 0;
	public static event System.Action onPhaseChange;
	public static event System.Action onGameStart;
	public static event System.Action onReversePhaseStart;
	public static event System.Action onGameOver;
	public static float phaseChangeTime = 8f;
	public static bool gameStarted = false;
	public static bool gameOvered = false;

	public float[] phaseLengths;
	float phaseStartTime;
	System.Action framewiseAction;

	// Use this for initialization
	void Start () {
		Cursor.visible = false;
		framewiseAction = TryStartGame;
	}
	
	// Update is called once per frame
	void Update () {
		if (framewiseAction != null) {
			framewiseAction();
		}
	}

	void TryStartGame() {
		if (!gameStarted) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				GameStart();
			}
			else if (Input.GetKeyDown(KeyCode.R)) {

				ReversePhaseStart();
			}
		}
	}

	void LateUpdate() {
		if (valueChangeEvent != null) {
			//valueChangeEvent(5);
		}
	}

	void ChangeDifficulty() {
		CheckEarlyReverse();
		if (Time.time - phaseStartTime > phaseLengths[difficulty]) {
			if (difficulty < 3) {
				// Normal phase change
				difficulty++;
				StageMusicCtrl.MoveNext();
				StageMusicCtrl.PlaySetup();
				Debug.Log ("Change to phase " + difficulty);
				if (onPhaseChange != null) 
					onPhaseChange();
				phaseStartTime = Time.time + phaseChangeTime;
				Invoke ("PlayStageWrapper", phaseChangeTime);
			}
			else if (difficulty == 3) {
				// Reverse phase start
				ReversePhaseStart();
			}
			else if (difficulty == 4) {
				// Game Over
				GameOver();
			}
		}
	}

	public void GameStart() {
		gameStarted = true;
		difficulty = 0;
		phaseStartTime = Time.time;
		framewiseAction = ChangeDifficulty;
		if (onGameStart != null) {
			onGameStart();
		}
		StageMusicCtrl.MoveNext();
		StageMusicCtrl.PlayStage();
		//onPhaseChange();
	}

	public static int RollOne() {
		int num = Random.Range(0,2);
		return (num == 0) ? 1 : -1;
	}

	void CheckEarlyReverse() {
		if (ProtegeController.staticHitPoints <= 0 && enemyInward) {
			ReversePhaseStart();
		}
	}

	void ReversePhaseStart() {
		gameStarted = true;
		enemyInward = false;
		difficulty = 4;
		framewiseAction = ChangeDifficulty;
		phaseStartTime = Time.time;
		StageMusicCtrl.JumpToFinal();
		StageMusicCtrl.PlayStage();
		if (onReversePhaseStart != null) 
			onReversePhaseStart();
	}

	public void GameOver() {
		gameOvered = true;
		if (onGameOver != null) {
			onGameOver();
		}
		StageMusicCtrl.MoveNext();
		StageMusicCtrl.PlayStage();
		framewiseAction = null;
		//Invoke("LoadLevel", 20f);
	}

	void PlayStageWrapper() {
		StageMusicCtrl.PlayStage();
	}

	void LoadLevel() {
		Application.LoadLevel(0);
	}
}
