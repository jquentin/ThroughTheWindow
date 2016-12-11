using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class StartLoopEndPlayer : TigglyMonoBehaviour
{

	public enum Mode { StartThenLoop, StartAndLoopTogether }
	public Mode mode;

	public enum State { NotPlaying, Start, Loop, End }
	public State currentState { get; private set; }

	public AudioClip StartClip;
	public AudioClip LoopClip;
	public AudioClip EndClip;

	private bool isPlaying;

	private AudioSource _secondAudioSource;
	public AudioSource secondAudioSource
	{
		get
		{
			if (_secondAudioSource == null)
				_secondAudioSource = gameObject.AddComponent<AudioSource>();
			return _secondAudioSource;
		}
	}

	void Awake()
	{
		currentState = State.NotPlaying;
	}

	public void StartPlaying()
	{
		isPlaying = true;
		StartCoroutine(play());
	}

	public void EndPlaying()
	{
		isPlaying = false;
	}

	private IEnumerator play()
	{
		if (StartClip != null)
		{
			
			secondAudioSource.clip = StartClip;
			secondAudioSource.loop = false;
			secondAudioSource.Play();
			currentState = State.Start;

			// If the mode is StartThenLoop, wait before going to the loop
			if (mode == Mode.StartThenLoop)
				yield return new WaitForSecondsOrUntil(StartClip.length, () => !isPlaying);
		}

		audioSource.clip = LoopClip;
		audioSource.loop = true;
		audioSource.Play();

		// If the mode is StartAndLoopTogether, stay in Start state
		if (mode == Mode.StartAndLoopTogether)
			yield return new WaitForSecondsOrUntil(StartClip.length, () => !isPlaying);
		
		currentState = State.Loop;

		yield return new WaitWhile(() => isPlaying);

		secondAudioSource.Stop();

		if (EndClip != null)
		{
			audioSource.clip = EndClip;
			audioSource.loop = false;
			audioSource.Play();
			currentState = State.End;

			yield return new WaitForSeconds(audioSource.clip.length);
		}

		audioSource.Stop();
		audioSource.clip = null;
		currentState = State.NotPlaying;
	}
}
