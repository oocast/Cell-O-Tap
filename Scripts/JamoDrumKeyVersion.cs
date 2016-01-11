//Jam-o-Drum unity interface
//Orignal version by Bryan Maher (27-Jan-2012)
//Updated by Andrew Roxby 2/19/12

using UnityEngine;
using System.Collections;
using ETC.Platforms;

public class JamoDrumKeyVersion : MonoBehaviour
{
	public int[] spinDelta = new int[4];
	public bool[] hit = new bool[4];
	
	private static JamoDrumClient jod = null;
	
	private ETC.Platforms.HitEventHandler hitEvents;
	private ETC.Platforms.SpinEventHandler spinEvents;
	
	void Start()
	{
		if(!Application.isEditor) Cursor.visible = false;
		if(jod==null) jod = JamoDrumClient.Instance;
		
		AddHitEvent(HandleJodHit);
		AddSpinEvent(HandleJodSpin);
		
		jod.Hit += CallHitEvents;
		jod.Spin += CallSpinEvents;
	}
	
	void CallHitEvents(int controllerID)
	{
		hitEvents(controllerID);
	}
	
	void CallSpinEvents(int controllerID, int delta)
	{
		spinEvents(controllerID, delta);
	}
	
	/// <summary>
	/// This is the code that is run when a pad is hit.
	/// </summary>
	/// This is a number from 1 to 4 indicating which controller
	/// is sending the message.
	/// </param>
	void HandleJodHit(int controllerID)
	{
		hit[controllerID - 1] = true;
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
		spinDelta[controllerID - 1] += delta;
	}
	
	public void AddHitEvent(HitEventHandler func)
	{
		hitEvents += func;
	}
	
	public void AddSpinEvent(SpinEventHandler func)
	{
		spinEvents += func;
	}
	
	public void InjectHit(int controllerID)
	{
		CallHitEvents(controllerID);
	}
	
	public void InjectSpin(int controllerID, int delta)
	{
		CallSpinEvents(controllerID, delta);
	}
	
	void LateUpdate()
	{	
		for(int i = 0; i < 4; i++)
		{
			spinDelta[i] = 0;
			hit[i] = false;
		}
	}	
}
