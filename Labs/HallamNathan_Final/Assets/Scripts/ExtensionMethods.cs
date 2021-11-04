using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
	/// <summary>
	///  ActivatesDeactivates the GameObject, depending on the given true or false/ value.
	/// </summary>
	/// <param name="value"> Activate or deactivate the object, where true activates the GameObject and false
	/// deactivates the GameObject.  </param>
	/// <returns></returns>
	public static void SetActive(this CanvasGroup CG, bool value)
	{
		if (value)
		{
			CG.alpha = 1;
			CG.interactable = true;
			CG.blocksRaycasts = true;
		}
		else
		{
			CG.alpha = 0;
			CG.interactable = false;
			CG.blocksRaycasts = false;
		}
	}

	public static void Toggle(this CanvasGroup CG)
	{
		if (CG.alpha == 0)
		{
			CG.alpha = 1;
			CG.interactable = true;
			CG.blocksRaycasts = true;
		}
		else
		{
			CG.alpha = 0;
			CG.interactable = false;
			CG.blocksRaycasts = false;
		}
	}

	/// <summary>
	///  Returns true if CanvasGroup is visible
	/// </summary>
	/// <returns></returns>
	public static bool activeInHierarchy(this CanvasGroup CG)
	{
		return (CG.alpha == 1 ? true : false);
	}
}
