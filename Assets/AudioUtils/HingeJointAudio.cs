using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HingeJointAudio : TigglyMonoBehaviour {

	public List<AudioClip> hingeSound; 

	public float minVelocity = 4f;
	public float maxVolumeVelocity = 10f;

	bool isMoving = false;

	void Update () 
	{
		float velocity = GetComponent<Rigidbody>().angularVelocity.magnitude;
		if (isMoving)
		{
			if (velocity < minVelocity)
				isMoving = false;
			return;
		}
		else
		{
			if (velocity >= minVelocity)
			{
				audioSource.PlayOneShotControlled(hingeSound, AudioType.Sound);
				isMoving = true;
			}
		}
		audioSource.volume = Mathf.Lerp(0.1f, 1f, Mathf.InverseLerp(minVelocity, maxVolumeVelocity, velocity));
	}
}
