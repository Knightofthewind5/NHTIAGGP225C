using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
	#region Canvas Group
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
	#endregion Canvas Group

	#region Color
	public static string ColorToHex(this Color32 color)
	{
		string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		return hex;
	}
	#endregion Color

	#region Camera
	public static bool ObjectIsVisible(this Camera camera, GameObject GO)
	{
		Vector3 horizontalOffset = camera.ViewportToWorldPoint(new Vector3(1.0f, 0.5f, -Camera.main.transform.position.z)); //Changed from v
		Vector3 verticalOffset = camera.ViewportToWorldPoint(new Vector3(0.5f, 1.0f, -Camera.main.transform.position.z)); //
		float horizontalEdge = horizontalOffset.x;
		float verticalEdge = verticalOffset.y;

		if (GO.transform.position.x > horizontalEdge)
		{
			return false;
		}
		else if (GO.transform.position.x < -horizontalEdge)
		{
			return false;
		}
		if (GO.transform.position.y > verticalEdge)
		{
			return false;
		}
		else if (GO.transform.position.y < -verticalEdge)
		{
			return false;
		}

		return true;
	}
	#endregion Camera
}
