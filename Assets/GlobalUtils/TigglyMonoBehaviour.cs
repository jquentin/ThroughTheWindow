using UnityEngine;
using System.Collections;

public class TigglyMonoBehaviour : MonoBehaviour {

	private AudioSource _audioSource;
	public AudioSource audioSource
	{
		get
		{
			if (_audioSource == null)
				_audioSource = this.GetOrAddComponent<AudioSource>();
			return _audioSource;
		}
	}

}
