using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamButton : MonoBehaviour {

	void Start()
	{
		Debug.Log(WebCamUITexture.instance.nbDevices);
		if (WebCamUITexture.instance.nbDevices <= 1)
			gameObject.SetActive(false);
	}

	void OnClick () 
	{
		WebCamUITexture.instance.SwitchCamera();
	}

}
