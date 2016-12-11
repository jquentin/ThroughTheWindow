using UnityEngine;
using System;
using System.Collections;

public class GameTimeout
{

	public Action OnTrigger;

	public float delay;

	public bool repeating;

	public Func<bool> predicate;

	bool isCanceled = false;

	public GameTimeout(float delay, Func<bool> predicate, Action callback, bool repeating)
	{
		this.delay = delay;
		this.predicate = predicate;
		this.OnTrigger += callback;
		this.repeating = repeating;
	}

	public IEnumerator Start () 
	{
		bool firstTime = true;
		while ((repeating || firstTime) && !isCanceled)
		{
			yield return new WaitForSecondsWhile(delay, predicate);
			if (isCanceled)
				break;
			if (OnTrigger != null)
				OnTrigger();
			firstTime = false;
		}
	}

	public void Cancel()
	{
		isCanceled = true;
	}

}

public static class TimeoutExtensions
{

	public static GameTimeout AddTimeout(this MonoBehaviour component, float delay, Func<bool> predicate, Action callback, bool repeating)
	{
		GameTimeout timeout = new GameTimeout(delay, predicate, callback, repeating);
		component.StartCoroutine(timeout.Start());
		return timeout;
	}

}
