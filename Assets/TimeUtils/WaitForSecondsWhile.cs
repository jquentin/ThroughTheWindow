using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Acts like a timeout behavior:
/// Finishes once the predicate has been true for delay seconds.
/// </summary>
public class WaitForSecondsWhile : CustomYieldInstruction {

	Func<bool> m_Predicate;

	float delay;

	Action action;

	float lastStartTime;

	public override bool keepWaiting 
	{ 
		get 
		{ 
			bool respectCondition = m_Predicate.Invoke();
			if (!respectCondition)
				InitializeTime();
			return Time.time < lastStartTime + delay;
		} 
	}

	public WaitForSecondsWhile(float delay, Func<bool> predicate)
	{
		this.delay = delay;
		m_Predicate = predicate;
		InitializeTime();
	}

	void InitializeTime()
	{
		lastStartTime = Time.time;
	}

}
