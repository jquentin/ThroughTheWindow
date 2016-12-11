using UnityEngine;
using System.Collections;

/// <summary>
/// This simple singleton class can be used to start a coroutine from a static environment.
/// Just call StaticCoroutineHandler.instance.StartCoroutine(your_static_coroutine).
/// </summary>
public class StaticCoroutineHandler : MonoBehaviour {

	static StaticCoroutineHandler _instance;
	public static StaticCoroutineHandler instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject go = new GameObject("StaticCoroutineHandler");
				_instance = go.AddComponent<StaticCoroutineHandler>();
			}
			return _instance;
		}
	}

}
