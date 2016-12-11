using UnityEngine;
using System;
using System.Collections;

public class WaitForSecondsOrUntil : CustomYieldInstruction {

	Func<bool> m_Predicate;

	float endTime;

	public override bool keepWaiting { get { return Time.time < endTime && !m_Predicate.Invoke(); } }

	public WaitForSecondsOrUntil(float seconds, Func<bool> predicate)
	{
		m_Predicate = predicate;
		endTime = Time.time + seconds;
	}

}
