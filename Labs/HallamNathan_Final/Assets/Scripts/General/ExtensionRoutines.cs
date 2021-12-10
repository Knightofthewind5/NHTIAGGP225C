using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtensionRoutines : MonoBehaviour
{
	// This will be null until after Awake()
	public static ExtensionRoutines Instance;

	private void Awake()
	{
		if (Instance)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	public Coroutine StartRoutine(IEnumerator firstIterationResult)
	{
		return StartCoroutine(firstIterationResult);
	}

	public void StopRoutine(IEnumerator firstIterationResult)
	{
		StopCoroutine(firstIterationResult);
	}

	public IEnumerator FadeCanvasGroup(CanvasGroup CG, float start, float end, float lerpTime = 2f)
	{
		float _timeStartLerping = Time.time;
		float timeSinceStarted = Time.time - _timeStartLerping;
		float percentageComplete = timeSinceStarted / lerpTime;
		float savedCurrentValue = CG.alpha;

		do
		{
			if (CG && CG.alpha != savedCurrentValue) break;
			if (CG == null) break;

			timeSinceStarted = Time.time - _timeStartLerping;
			percentageComplete = timeSinceStarted / lerpTime;

			float currentValue = Mathf.Lerp(start, end, percentageComplete);
			savedCurrentValue = currentValue;

			CG.alpha = currentValue;

			if (percentageComplete >= 1) break;

			yield return new WaitForEndOfFrame();
		} while (true);
	}
}
