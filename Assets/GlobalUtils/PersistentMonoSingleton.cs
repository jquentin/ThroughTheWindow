using UnityEngine;
using System.Collections;

public abstract class PersistentMonoSingleton<T> : MonoSingleton<T> where T : MonoSingleton<T>
{

	protected virtual void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

}
