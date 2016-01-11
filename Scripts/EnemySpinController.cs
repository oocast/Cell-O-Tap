using UnityEngine;
using System.Collections;

public class EnemySpinController : MonoBehaviour {
	public bool spin;
	public bool hasAnime;
	float spinSpeed = 90f;

	// normal, shock, dead
	public Sprite[] sprites;
	int spriteIndex = 0;
	bool damage = false;

	// Use this for initialization
	System.Action normalAnim;

	void Start () {
		StartCoroutine("ChangeSprite");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {
		if (spin) {
			Spin ();
		}
	}

	void Spin() {
		transform.Rotate(spinSpeed * Time.fixedDeltaTime * Vector3.forward);
	}

	void FrameAnime() {
	}

	IEnumerator ChangeSprite() {
		while(true) {
			if (damage) {
				GetComponent<SpriteRenderer>().sprite = sprites[spriteIndex];
				spriteIndex = spriteIndex == 1 ? 2 : 1;
			}
			else if (spriteIndex != 0) {
				spriteIndex = 0;
				GetComponent<SpriteRenderer>().sprite = sprites[0];
				if (hasAnime) {
					GetComponent<SpriteRenderer>().enabled = false;
					transform.GetChild(0).gameObject.SetActive(true);
				}
			}
			damage = false;
			yield return new WaitForSeconds(0.1f);
		}
	}

	public void OffDamage() {
		GetComponent<SpriteRenderer>().sprite = sprites[0];
		damage = false;
	}

	public void OnDamage() {
		damage = true;
		if (hasAnime) {
			GetComponent<SpriteRenderer>().enabled = true;
			transform.GetChild(0).gameObject.SetActive(false);
		}
		if (spriteIndex == 0) spriteIndex = 1;
	}

	public void OnDie() {
		StopCoroutine("ChangeSprite");
		if (hasAnime) {
			GetComponent<SpriteRenderer>().enabled = true;
			transform.GetChild(0).gameObject.SetActive(false);
		}
		GetComponent<SpriteRenderer>().sprite = sprites[2];
	}
}
