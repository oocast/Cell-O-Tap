using UnityEngine;
using System.Collections;

public class ProtegeController : MonoBehaviour {
	public System.Action action = null;
	public int hitpoints = 100;
	public static int staticHitPoints;
	public GameObject[] stages;
	bool hurting;

	// Use this for initialization
	void Start () {
		//GameController.onPhaseChange += InvokeHealCell;
		stages [1].SetActive (false);
		stages [0].SetActive (true);
		stages [2].SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		staticHitPoints = hitpoints;
	}

	void OnDamage(int value) {
		// Play animation of get damage	
		hitpoints -= value;
		AudioManager.PlayAudio ("hurt0", Vector3.zero);
		Debug.Log("Cell gets damage! Current HP " + hitpoints);
		if (!hurting) {
			hurting = true;
			stages [1].SetActive (true);
			stages [0].SetActive (false);
			Invoke("Recover", 0.5f);
		}
	}

	void Recover() {
		hurting = false;
		stages [1].SetActive (false);
		stages [0].SetActive (true);
	}

	void OnTriggerEnter(Collider other) {

		if (other.tag == "Enemy" && GameController.enemyInward) {
			OnDamage (other.GetComponent<Enemy> ().OnAttack ());
		} else if (other.attachedRigidbody)
			if (other.attachedRigidbody.CompareTag ("Beam") && (GameController.enemyInward)) {
				if (GameController.gameStarted)
					OnDamage(other.attachedRigidbody.GetComponent<BeamCtrl> ().centerAttack);
			}
		if (!GameController.enemyInward) {
			
			stages [1].SetActive (false);
			stages [0].SetActive (false);
			stages [2].SetActive (true);
		}
	}

	void InvokeHealCell() {
		Invoke ("HealCell", GameController.phaseChangeTime);
	}

	void HealCell() {
		hitpoints = 100;
	}
}
