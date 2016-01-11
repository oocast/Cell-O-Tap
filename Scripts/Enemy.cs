using UnityEngine;
using System.Collections;

public enum MovingPattern {
	STRAIGHT,
	ZIGZAG,
	SINEWAVE,
	SPIRAL,
	REVERSE
};

public class Enemy : MonoBehaviour {

	public float angle;
	public MovingPattern movingPattern;
	public float speed = 3f;
	public float hitpoints = 100;
	public int attack = 5;
	public GameObject deathParticle;

	// how much an enemy is worth
	int score;

	// if ZigZag or Sine or Spiral
	public float amplitude = 4f;
	public float slopeOrFreq = 3f;
	float birthTime;
	Transform center;
	bool noDamage = false;

	Vector3 centerLine;
	Vector3 right;
	Vector3 forward;

	System.Action action;
	PlayersController players;
	Vector3 circleCenter;
	float circleRadius;
	Rigidbody rb;

	bool hurting;

	// Use this for initialization
	void Start () {
		//Initialize(0f, movingPattern);
		birthTime = Time.time;
		action += Proceed;
		players = GameObject.Find ("Players").GetComponent<PlayersController>();
		center = GameObject.Find("Center").transform;
		rb = GetComponent<Rigidbody>();
	}

	// must be called
	public void Initialize(float angle, 
	                       MovingPattern moving, 
	                       int region,
	                       float speed, 
	                       float amplitude,
	                       float hitpoints,
	                       int attack,
	                       int score){
		Relocation2D(Vector3.zero);
		transform.Translate(transform.right * GameController.tableRadius, Space.World);
		transform.RotateAround(Vector3.zero, transform.up, angle + 90f * region);
		LookAt2D(Vector3.zero);
		centerLine = transform.position;
		this.speed = speed;
		this.amplitude = amplitude;
		this.hitpoints = hitpoints;
		this.attack = attack;
		this.score = score;
		movingPattern = moving;
		right = transform.right;
		forward = transform.forward;
	}

	public void Initialize(EnemyBirth birth){
		Relocation2D(birth.origin);
		transform.Translate(Vector3.right * birth.radius, Space.World);
		transform.RotateAround(Vector3.zero, transform.up, birth.angle + 90f * birth.region);
		LookAt2D(Vector3.zero);
		centerLine = transform.position;
		speed = birth.speed;
		amplitude = birth.amplitude;
		hitpoints = birth.hitpoints;
		attack = birth.attack;
		score = birth.score;
		movingPattern = birth.movingPattern;
		right = transform.right;
		forward = transform.forward;
		if (GameController.enemyInward) {
			center = GameObject.Find("Center").transform;
		}
		else {
			Transform target = GameObject.Find ("Drum" + Random.Range (1,5)).transform;
			circleCenter = 0.5f * (target.position + birth.origin);
			circleCenter.y = 0f;
			LookAt2D(circleCenter);
			int dir = GameController.RollOne();
			rb = GetComponent<Rigidbody>();
			rb.AddForce(dir * speed * transform.right, ForceMode.VelocityChange);
			circleRadius = (birth.origin - circleCenter).magnitude;
			noDamage = true;
			Invoke ("ResetNoDamage", 2f);
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (action != null) {
			action();
		}
	}

	void Proceed() {
		float centripetalMove = speed * Time.fixedDeltaTime;
		float tangentialMove;
		forward = center.position - centerLine;
		forward.y = 0;
		forward.Normalize();
		right = Vector3.Cross(transform.up, forward);
		centerLine += forward * centripetalMove;


		switch(movingPattern) {
		case MovingPattern.STRAIGHT:
			transform.position = centerLine;
			break;
		case MovingPattern.ZIGZAG:
			// tangentialMove is SPEED!
			tangentialMove = slopeOrFreq * centripetalMove;
			transform.position += forward * centripetalMove + right * tangentialMove;
			if (Vector3.Distance(transform.position, centerLine) > amplitude * 1.5f) {
				slopeOrFreq = -slopeOrFreq;
			}
			break;
		case MovingPattern.SINEWAVE:
			// tangentialMove is DISTANCE!
			tangentialMove = amplitude * Mathf.Sin(slopeOrFreq * (Time.time - birthTime));
			Vector3 position = centerLine + right * tangentialMove;
			transform.position = position;
			break;
		case MovingPattern.SPIRAL:
			// tangentialMove is ANGULAR SPEED!
			tangentialMove = Mathf.Rad2Deg * speed * Time.fixedDeltaTime / amplitude;
			transform.RotateAround(center.position, transform.up, tangentialMove);
			LookAt2D(center.position);
			transform.position += transform.forward * centripetalMove;
			break;
		case MovingPattern.REVERSE:
			//Vector3 direction = Vector3.Cross(Vector3.up, rb.velocity);
			Vector3 direction = circleCenter - transform.position;
			direction.y = 0;
			//float r = direction.magnitude;
			direction.Normalize();
			//float check = Vector3.Dot(direction, circleCenter - transform.position)
			rb.AddForce((speed * speed / circleRadius) * direction, ForceMode.Acceleration);
			break;
		default:
			Debug.Log("Undefined moving pattern");
			break;
		}
		LookAt2D(center.position);

	}

	void ResetHurting() {
		hurting = false;
	}

	public void GetDamage(float value, int left, int right) {
		//Debug.Log("Get Damage " + value);
		if (hitpoints < 0 || noDamage) return;
		GetComponentInChildren<EnemySpinController>().OnDamage();
		hitpoints -= value;
		if ((!hurting) && (GameController.enemyInward)) {
			hurting = true;
			AudioManager.PlayAudio ("vHurt", transform.position);
			Invoke ("ResetHurting", AudioManager.GetAudio("vHurt").length * 0.5f);
		}
		if (hitpoints < 0) {
			OnDie ();
			players.AddScore(score, left, right);
		}
	}

	public void GetDamage(float value, int index) {
		//Debug.Log("Get Damage " + value);
		if (hitpoints < 0 || noDamage) return;
		GetComponentInChildren<EnemySpinController>().OnDamage();
		hitpoints -= value;
		AudioManager.PlayAudio ("vHurt", transform.position);

		if (hitpoints < 0) {
			OnDie ();
			players.AddScore(score, index);
		}
	}

	void Attack() {
		// Play enemy animation when attacking
	}

	void Die() {
		// Play enemy dying animation
	}

	// return enemy strength and change delegate
	public int OnAttack() {
		action -= Proceed;
		action += Attack;
		if (GameController.enemyInward) {
			transform.parent = center;
		}
		Destroy(gameObject, 1f);
		noDamage = true;
		//Debug.Log ("Enemy start playing attacking animation");
		return attack;
	}

	void ResetNoDamage() {
		noDamage = false;
	}

	// change delegate
	public void OnDie() {
		action -= Proceed;
		action += Die;
		GetComponentInChildren<EnemySpinController>().OnDie();
		var death = Instantiate(deathParticle);
		death.transform.position = transform.position;
		if (!GameController.gameOvered)
			AudioManager.PlayAudio("vDie", transform.position);
		Destroy(gameObject, 1f);
		GameController.numDeadEnemy++;
		//Debug.Log ("Enemy start playing dying animation");
	}

	void LookAt2D(Vector3 pivet) {
		pivet.y = transform.position.y;
		transform.LookAt(pivet);
	}
	
	void Relocation2D(Vector3 target) {
		target.y = transform.position.y;
		transform.position = target;
	}
}
