//Exmaple Jam-o-Drum simulator
//Andrew Roxby 2/29/12

using UnityEngine;
using System.Collections;
using ETC.Platforms;

public class JamoKey : MonoBehaviour
{
	private KeyCode[] spinLeft = {KeyCode.A, KeyCode.Keypad7, KeyCode.Keypad4, KeyCode.Keypad1};
	private KeyCode[] spinRight = {KeyCode.D, KeyCode.Keypad9, KeyCode.Keypad6, KeyCode.Keypad3};
	private KeyCode[] hit = {KeyCode.S, KeyCode.Keypad8, KeyCode.Keypad5, KeyCode.Keypad2};
	
	public JamoDrumKeyVersion jamoDrum;
	public int spinsPerFrame;
	
	void Update()
	{
		if(jamoDrum==null) return;
		for(int i = 0; i<4; i++)
		{
			if(Input.GetKey(spinLeft[i])) jamoDrum.InjectSpin(i+1, -spinsPerFrame);
			if(Input.GetKey(spinRight[i])) jamoDrum.InjectSpin(i+1, spinsPerFrame);
			if(Input.GetKeyDown(hit[i])) jamoDrum.InjectHit(i+1);
		}
	}
}
