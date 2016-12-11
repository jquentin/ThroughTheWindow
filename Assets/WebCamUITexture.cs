using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebCamUITexture : MonoSingleton <WebCamUITexture> {

	UITexture _uiTexture;
	UITexture uiTexture
	{
		get
		{
			if (_uiTexture == null)
				_uiTexture = GetComponent<UITexture>();
			return _uiTexture;
		}
	}
	WebCamDevice[] _devices;
	WebCamDevice[] devices
	{
		get
		{
			if (_devices == null)
				_devices = WebCamTexture.devices;
			return _devices;
		}
	}

	public int nbDevices
	{
		get
		{
			return devices.Length;
		}
	}

	int currentCamIndex = 0;

	void Start () 
	{
		currentCamIndex = 0;
		SwitchToCurrentCamera();
	}

	public void SwitchCamera () 
	{
		currentCamIndex = (currentCamIndex + 1) % devices.Length;

		SwitchToCurrentCamera();
	}

	void SwitchToCurrentCamera()
	{
		WebCamTexture webcamTexture = new WebCamTexture(devices[currentCamIndex].name);
		uiTexture.mainTexture = webcamTexture;
		webcamTexture.Play();
	}
}
