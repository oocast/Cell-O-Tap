using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemyBirth : System.Object {
	public Vector3 origin;
	public float radius;
	public float angle;
	// 0~3, region * 90f degrees + angle
	public int region;
	public MovingPattern movingPattern;
	public float speed;
	public float amplitude;
	public float hitpoints;
	public int attack;
	public int score;

	public void MultipliedBy(LevelModifier modifier) {
		speed *= modifier.speed;
		amplitude *= modifier.amplitude;
		hitpoints *= modifier.hitpoint;
		attack =(int)(attack * modifier.attack);
		score *= modifier.score;
		movingPattern = modifier.movingPattern;
	}
}

[System.Serializable]
public class LevelModifier : System.Object {
	public float speed;
	public float hitpoint;
	public float attack;
	public float amplitude;
	public int score;
	public GameObject prefab;
	public MovingPattern movingPattern;
}

public class EnemyGenerator : MonoBehaviour {
	
	public GameObject enemies;
	public LevelModifier[] levelModifiers;
	public float genAngle = 20f;
	public float minSpeed = 2f;
	public float maxSpeed = 3f;
	public float minAmplitude = 2f;
	public float maxAmplitude = 4f;
	public float baseHp = 100;
	const int basicAttack = 5;
	const int basicScore = 1;
	
	public int minNumEnemy = -20;
	public int[] maxNumEnemyByPhase;
	public float coolDownTime = 2f;
	bool genCoolDown = false;
	float pauseStartTime;
	System.Action generateAction;
	bool pauseGenerate = true;

	Transform cell;

	// Use this for initialization
	void Start () {
		//Invoke("RegularGenerate", Random.Range (1f, 3f));
		//RegularGenerate();
		cell = GameObject.Find("Center").transform;
		GameController.onPhaseChange += PauseGenerate;
		GameController.onGameStart += UnlockGenerator;
		GameController.onReversePhaseStart += StartReverseGenerate;
		GameController.onGameOver += StopReverseGenerate;
		generateAction = RegularGenerateWrapper;
	}

	
	// Update is called once per frame
	void Update () {
		// TrySpecificGenerate();
		if (generateAction != null) {
			generateAction();
		}
	}

	void UnlockGenerator() {
		pauseGenerate = false;
	}

	int RollEnemyLevel() {
		int minLevel, maxLevel, resultLevel;
		switch(GameController.difficulty) {
		case 0:
			minLevel = 0;
			maxLevel = 1;
			break;
		case 1:
			minLevel = 1;
			maxLevel = 3;
			break;
		case 2:
			minLevel = 3;
			maxLevel = 4;
			break;
		case 3:
			minLevel = 0;
			maxLevel = 4;
			break;
		case 4:
			minLevel = 4;
			maxLevel = 7;
			break;
		default:
			minLevel = maxLevel = 0;
			break;
		}
		resultLevel = Random.Range(minLevel, maxLevel);
		return resultLevel;
	}

	int RollEnemyNumber() {
		int maxNumEnemy = maxNumEnemyByPhase[GameController.difficulty];
		int numEnemy = Random.Range (minNumEnemy, maxNumEnemy);
		if (GameController.difficulty == 3 && numEnemy > maxNumEnemy - 4) {
			maxNumEnemyByPhase[3] += 2;
		}
		return numEnemy;
	}

	void RegularGenerateWrapper() {
		if (!genCoolDown && !pauseGenerate) {
			genCoolDown = true;
			Invoke("RegularGenerate", coolDownTime);
		}
	}

	void RegularGenerate() {
		int numEnemy = RollEnemyNumber();
		if (numEnemy > 0) {
			StartCoroutine("GenerateCoroutine", numEnemy);
		}
		else if (!pauseGenerate){
			Invoke ("RegularGenerate", 0f);
		}
	}

	IEnumerator GenerateCoroutine(int numEnemy) {
		for (int i = 0; i < numEnemy && !pauseGenerate; i++) {
			//Enemy.MovingPattern randMoving = (Enemy.MovingPattern)Random.Range(0, System.Enum.GetValues(typeof(Enemy.MovingPattern)).Length);
			//Debug.Log (randMoving);
			
			GenerateEnemy(
				Random.Range(-genAngle, genAngle),
				MovingPattern.STRAIGHT, //Placeholder
				Random.Range (0,4),
				Random.Range (minSpeed, maxSpeed),
				Random.Range (minAmplitude, maxAmplitude),
				baseHp,
				basicAttack,
				basicScore
				);
			AudioManager.PlayAudio("e" + Random.Range(0,22), Vector3.zero);
			yield return new WaitForSeconds(Random.Range(0f, 0.6f));
		}
		//Debug.Log("Generate " + numEnemy + " Enemies");
		genCoolDown = false;
		yield return null;
	}

	void GenerateEnemy(float angle = 0f, 
	                   MovingPattern moving = MovingPattern.STRAIGHT, 
	                   int region = 0,
	                   float speed = 3f, 
	                   float amplitude = 4f,
	                   float hitpoints = 100,
	                   int attack = basicAttack,
	                   int score = basicScore) {
		int enemyLevel = RollEnemyLevel();
		var modifier = levelModifiers[enemyLevel];
		var enemy = Instantiate(modifier.prefab).GetComponent<Enemy>();
		enemy.transform.parent = enemies.transform;
		enemy.Initialize(angle,
		                 modifier.movingPattern, 
		                 region, 
		                 speed * modifier.speed, 
		                 amplitude * modifier.amplitude, 
		                 (int)(hitpoints * modifier.hitpoint),
		                 (int)(attack * modifier.attack),
		                 score * modifier.score);
	}

	void PauseGenerate() {
		pauseStartTime = Time.time;
		generateAction = PauseGenerateWrapper;
		genCoolDown = true;
		pauseGenerate = true;
	}

	void PauseGenerateWrapper() {
		if (Time.time - pauseStartTime > GameController.phaseChangeTime) {
			generateAction = RegularGenerateWrapper;
			genCoolDown = false;
			pauseGenerate = false;
		}
	}

	void StartReverseGenerate() {
		pauseGenerate = true;
		generateAction = null;
		StartCoroutine("ReverseGenerate");
	}

	void StopReverseGenerate() {
		StopCoroutine("ReverseGenerate");
	}

	IEnumerator ReverseGenerate() {
		while (true) {
			if (!GameController.enemyInward) {
				for (int i = 0; i < 4; i++) {
					EnemyBirth birth = new EnemyBirth();
					birth.origin = cell.position;
					birth.attack = basicAttack;
					birth.score = basicScore;
					birth.speed = maxSpeed;
					birth.hitpoints = 100;
					birth.movingPattern = MovingPattern.REVERSE;
					birth.radius = 0f;
					birth.angle = 0f;
					birth.region = 0;
					ReverseGenerateEnemy(birth);
				}
				yield return new WaitForSeconds(0.3f);
			}
		}
	}

	void ReverseGenerateEnemy(EnemyBirth birth) {
		int enemyLevel = RollEnemyLevel();
		var modifier = levelModifiers[enemyLevel];
		var enemy = Instantiate(modifier.prefab).GetComponent<Enemy>();
		enemy.transform.parent = enemies.transform;
		birth.MultipliedBy(modifier);
		enemy.Initialize(birth);
	}
}
