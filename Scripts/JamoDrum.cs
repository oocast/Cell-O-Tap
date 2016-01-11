 using UnityEngine;
using System.Collections;
using ETC.Platforms;

public class JamoDrum : MonoBehaviour {
	
	/// <summary>
	/// This is an instance of the JamoDrumClient used to receive
	/// user input.  DO NOT make more than one instance of the
	/// JamoDrumClient.  It just occurred to me that JamoDrumClient
	/// should be a Singleton.  I will make that change, expect it
	/// to be coming soon.
	/// 
	/// Bryan (27-Jan-2012)
	/// </summary>
	private JamoDrumClient jod;
	
	public int[] spinDelta = new int[4];
	public bool[] hit = new bool[4];
	
	// Use this for initialization
	void Start () {
		if(!Application.isEditor) {
			Cursor.visible = false;
		}
		jod = JamoDrumClient.Instance;
		jod.Hit += HandleJodHit;
		jod.Spin += HandleJodSpin;
	}

	/// <summary>
	/// This is the code that runs when any one of the
	/// spinners is rotated.
	/// </summary>
	/// <param name="controllerID">
	/// This is a number from 1 to 4 indicating which controller
	/// is sending the message.
	/// </param>
	/// <param name="delta">
	/// This is how much the spinner changed since the last event.
	/// The value is directly from the underlying mouse hardware,
	/// there is no "unit" for this number.  That is, it does not
	/// represent degrees of rotation, or millimeters of movement,
	/// it's just a relative number.
	/// In the future, we may want to have a calibration routine
	/// which determines how many delta values there are per
	/// rotation of the spinner.  This would allow us to convert
	/// the number to degrees of rotation.
	/// </param>
	void HandleJodSpin(int controllerID, int delta)
	{
		//Debug.Log("Spin!");		
	
		switch (controllerID)
		{
			case 1:
				// Station 1 rotates the cube around the X axis.
				spinDelta[0] += delta;					
				break;
			case 2:
				// Station 2 rotates the cube around the Y axis.
				spinDelta[1] += delta;
				break;
			case 3:
				// Station 3 rotates the cube around the Z axis.
				spinDelta[2] += delta;
				break;
			case 4:
				// There aren't 4 axes so station 4 gets to rotate them all.
				spinDelta[3] += delta;
				break;
		}		
	
	}
	
	/// <summary>
	/// This is the code that is run when a pad is hit.
	/// </summary>
	/// This is a number from 1 to 4 indicating which controller
	/// is sending the message.
	/// </param>
	void HandleJodHit(int controllerID)
	{
		// Nothing is being done with pad hits other than logging them.
		// You can check the log file in the EXE directory to see them
		// show up.  No, really, you can.
		Debug.Log("Hit!");
		
		switch (controllerID)
		{
			case 1:
				// Station 1 rotates the cube around the X axis.
				hit[0] = true;					
				break;
			case 2:
				// Station 2 rotates the cube around the Y axis.
				hit[1] = true;
				break;
			case 3:
				// Station 3 rotates the cube around the Z axis.
				hit[2] = true;
				break;
			case 4:
				// There aren't 4 axes so station 4 gets to rotate them all.
				hit[3] = true;
				break;
		}	
	}
	
	public void AddSpinEvent(SpinEventHandler func) {
		jod.Spin += func;
	}
	
	public void AddHitEvent(HitEventHandler func) {
		jod.Hit += func;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{	
		for(int i = 0; i < 4; i++) {
			spinDelta[i] = 0;
			hit[i] = false;
		}
	}
}
