using UnityEngine;
using System.Collections;

public abstract class MonoSingleton<T> : TigglyMonoBehaviour where T : MonoSingleton<T>
{
	
	protected static T _instance = null;
	public static T instance
	{
		get
		{
			if( _instance == null )
			{
				_instance = GameObject.FindObjectOfType(typeof(T)) as T;
			}
			return _instance;
		}
	}

	protected virtual void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}
}
