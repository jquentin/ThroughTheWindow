using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTextureButton : MonoBehaviour {

	void OnClick () 
	{
		CondensationTexture.instance.SwitchTexture();
	}

}
