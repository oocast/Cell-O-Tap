using UnityEngine;
using System.Collections;

public class JamODrumExample : MonoBehaviour {
	
	public JamoDrum jod;
	public JamoDrumKeyVersion jodk;
	public bool key = false;
	
	public GameObject[] spinners = new GameObject[4];
	public Material[] starMaterials = new Material[4];
	public GameObject star;
	public float[] degPerTick = new float[4];
	public float[] spinnerAngle = new float[4];
	private float[] initAngle = new float[4];
	public bool[] hits = new bool[4];

	private bool once;
	
	// Use this for initialization
	void Start () {
		for(int i=0; i<4; i++) {
			initAngle[i] = spinners[i].transform.rotation.eulerAngles.y;
			hits[i] = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!key) {
			if (!once) {
				once = true;
				jod.AddHitEvent (HitHandler);
				jod.AddSpinEvent (SpinHandler);
			}
			for (int i=0; i<4; i++) {
				//spin
				hits[i] = jod.hit[i];
				if (Mathf.Abs (jod.spinDelta [i]) > 0) {
					Debug.Log ("EXAMPLE SPIN " + i);
					spinnerAngle [i] += jod.spinDelta [i] * degPerTick [i];
					/*
					if (spinnerAngle[i] > 45.0f) {
						spinnerAngle[i] = 45.0f;
					}
					if (spinnerAngle[i] < -45.0f) {
						spinnerAngle[i] = -45.0f;
					}
					*/
					//spinnerAngle[i] += jod.spinDelta[i];
					Debug.Log ("Spin " + i + " : " + jod.spinDelta [i]);
					spinnerAngle [i] = Mathf.Repeat (spinnerAngle [i], 360);
					Vector3 rot = spinners [i].transform.rotation.eulerAngles;
					rot.y = initAngle [i] + spinnerAngle [i];
					spinners [i].transform.rotation = Quaternion.Euler (rot);
				}
				//hit
				if (jod.hit [i]) {
					Debug.Log ("EXAMPLE HIT " + i);
					/*
					GameObject starInst = (GameObject)Instantiate (star);
					starInst.GetComponent<Renderer> ().material = starMaterials [i];
					switch (i) {
					case 0:
						starInst.transform.position = new Vector3 (-5, 35, 5);
						break;
					case 1:
						starInst.transform.position = new Vector3 (5, 35, 5);
						break;
					case 2:
						starInst.transform.position = new Vector3 (5, 35, -5);
						break;
					case 3:
						starInst.transform.position = new Vector3 (-5, 35, -5);
						break;
					}
					*/
				}
			}
		
			if (Input.GetKeyUp (KeyCode.Escape)) {
				Application.Quit ();
			}
		} else {
			if (!once) {
				once = true;
				jodk.AddHitEvent (HitHandler);
				jodk.AddSpinEvent (SpinHandler);
			}
			for (int i=0; i<4; i++) {
				//spin
				hits[i] = jodk.hit[i];
				if (Mathf.Abs (jodk.spinDelta [i]) > 0) {
					Debug.Log ("EXAMPLE SPIN " + i);
					spinnerAngle [i] += jodk.spinDelta [i] * degPerTick [i];
					//spinnerAngle[i] += jod.spinDelta[i];
					Debug.Log ("Spin " + i + " : " + jodk.spinDelta [i]);
					/*
					if (spinnerAngle[i] > 45.0f) {
						spinnerAngle[i] = 45.0f;
					}
					if (spinnerAngle[i] < -45.0f) {
						spinnerAngle[i] = -45.0f;
					}
					*/
					spinnerAngle [i] = Mathf.Repeat (spinnerAngle [i], 360);
					Vector3 rot = spinners [i].transform.rotation.eulerAngles;
					rot.y = initAngle [i] + spinnerAngle [i];
					spinners [i].transform.rotation = Quaternion.Euler (rot);
				}
				//hit
				if (jodk.hit [i]) {
					Debug.Log ("EXAMPLE HIT " + i);
					/*
					GameObject starInst = (GameObject)Instantiate (star);
					starInst.GetComponent<Renderer> ().material = starMaterials [i];
					switch (i) {
					case 0:
						starInst.transform.position = new Vector3 (-5, 35, 5);
						break;
					case 1:
						starInst.transform.position = new Vector3 (5, 35, 5);
						break;
					case 2:
						starInst.transform.position = new Vector3 (5, 35, -5);
						break;
					case 3:
						starInst.transform.position = new Vector3 (-5, 35, -5);
						break;
					}
					*/
				}
			}
			
			if (Input.GetKeyUp (KeyCode.Escape)) {
				Application.Quit ();
			}
		}
	}
	
	public void SpinHandler(int controllerID, int delta) {
		Debug.Log("SPIN EVENT "+(controllerID-1));
	}
		
	public void HitHandler(int controllerID) {
		Debug.Log("HIT EVENT "+(controllerID-1));
	}
}
