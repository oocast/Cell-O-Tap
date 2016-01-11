using UnityEngine;
using System.Collections;


public class publicTest : MonoBehaviour 
{   
	delegate int MyDelegate(int num);
	MyDelegate myDelegate;
	int hp = 100;
	
	void Start () 
	{

	}

	void Update() {
		GameController.valueChangeEvent += ChangeHp;
		int a = 100;
		float b = 1.5f;
		Debug.Log((int)(a * b));
	}

	void ChangeHp(int value) {
		hp += value;
		//Debug.Log (gameObject.name + "\t" + hp);
		GameController.valueChangeEvent -= ChangeHp;
	}
}