using UnityEngine;
using System.Collections;

public abstract class ResourcePrefabMonoSingleton<T> : TigglyMonoBehaviour where T : ResourcePrefabMonoSingleton<T>
{
	
	protected static T _instance = null;
	public static T instance
	{
		get
		{
			if( _instance == null )
			{
				string name = typeof(T).Name;
				_instance = Instantiate<T>(Resources.Load<T>(name));
				if( _instance == null )
				{
					Debug.LogWarning("The prefab for ResourceSingleton: " + name + " was not found in Resources.");
				}
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

	protected virtual void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
}
