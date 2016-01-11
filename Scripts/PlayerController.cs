using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public int hitpoints = 100;
	int score = 0;
	int combo = 0;

	float scoreTime = 0f;
	const float comboPeriod = 3f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		ClearCombo();
	}

	public void GetDamage(int damage) {
		hitpoints -= damage;
	}

	public void AddScore(int deltaScore) {
		combo++;
		if (GameController.enemyInward) {
			score += deltaScore * combo;
		}
		else {
			score += deltaScore;
		}
		scoreTime = Time.time;
	}

	void ClearCombo() {
		if (combo != 0 && Time.time - scoreTime > comboPeriod) {
			combo = 0;
		}
	}

	public int GetScore() {
		return score;
	}

	public string GetScoreInfo() {
		return "Score: " + score + "\tCombo: " + combo;
	}


	void OnDamage(int value) {
		// Play animation of get damage	
		score -= value;
		Debug.Log("Cell gets damage! Current HP " + hitpoints);
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Enemy" && !GameController.enemyInward) {
			OnDamage(other.GetComponent<Enemy>().OnAttack());
		}
	}
}
