using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtensionRoutines : MonoBehaviour
{
	// This will be null until after Awake()
	public static ExtensionRoutines Instance = null;

	private void Awake()
	{
		Instance = this;
	}

	public Coroutine StartRoutine(IEnumerator firstIterationResult)
	{
		return StartCoroutine(firstIterationResult);
	}

	public IEnumerator FadeCanvasGroup(CanvasGroup CG, float start, float end, float lerpTime = 2f)
	{
		float _timeStartLerping = Time.time;
		float timeSinceStarted = Time.time - _timeStartLerping;
		float percentageComplete = timeSinceStarted / lerpTime;

		do
		{
			timeSinceStarted = Time.time - _timeStartLerping;
			percentageComplete = timeSinceStarted / lerpTime;

			float currentValue = Mathf.Lerp(start, end, percentageComplete);

			CG.alpha = currentValue;

			if (percentageComplete >= 1) break;

			yield return new WaitForEndOfFrame();
		} while (true);
	}
}
