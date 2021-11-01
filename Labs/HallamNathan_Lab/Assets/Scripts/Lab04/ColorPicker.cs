using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

[Serializable]
public class ColorEvent : UnityEvent<Color> { }

public static class CanvasExtensionMethods
{
	//https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/recommended-tags
	/// <summary>
	///  ActivatesDeactivates the GameObject, depending on the given true or false/ value.
	/// </summary>
	/// <param name="value"> Activate or deactivate the object, where true activates the GameObject and false
	/// deactivates the GameObject.  </param>
	/// <returns></returns>
	public static void SetActive(this CanvasGroup CG, bool value)
	{
		if (value == true)
		{
			CG.alpha = 1;
			CG.interactable = true;
			CG.blocksRaycasts = true;
		}
		else if (value == false)
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
	public static bool IsActive (this CanvasGroup CG)
	{
		if (CG.alpha == 1)
		{
			return true;
		}

		return false;
	}
}

//https://www.youtube.com/watch?v=rKhFYxUNL6A
public class ColorPicker : MonoBehaviour
{
	public TextMeshProUGUI DebugText;
	public ColorEvent OnColorPreview;
	public ColorEvent OnColorSelect;
	public GameObject Selector;
	RectTransform Rect;
	Texture2D ColorTexture;
	CanvasGroup group;

	private void Start()
	{
		Rect = GetComponent<RectTransform>();

		ColorTexture = GetComponentInChildren<Image>().mainTexture as Texture2D;

		group = gameObject.GetComponent<CanvasGroup>();
	}

	private void Update()
	{
		if (group.IsActive() && RectTransformUtility.RectangleContainsScreenPoint(Rect, Input.mousePosition))
		{
			Vector2 delta;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(Rect, Input.mousePosition, null, out delta);

			string debug = "mousePosition = " + Input.mousePosition;
			debug += "<br>delta=" + delta;

			//Get coordinate of Rectangle start with (0, 0) at bottom left of Rect
			float width = Rect.rect.width;
			float height = Rect.rect.height;
			delta += new Vector2(width * 0.5f, height * 0.5f);
			debug += "<br>offsetDelta=" + delta;

			//Get 0 to 1 coordinate of Rectangle
			float x = Mathf.Clamp(delta.x / width, 0, 1);
			float y = Mathf.Clamp(delta.y / height, 0, 1);
			debug += "<br>x=" + x + " y=" + y;

			//Get 1024x1024 coordinate of Rectangle
			int textX = Mathf.RoundToInt(x * ColorTexture.width);
			int textY = Mathf.RoundToInt(y * ColorTexture.height);
			debug += "<br>textX=" + textX + " textY=" + textY;

			Color color = ColorTexture.GetPixel(textX, textY);

			DebugText.color = color;
			DebugText.text = debug;

			Selector.transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) ;

			OnColorPreview?.Invoke(color);

			if (Input.GetMouseButtonDown(0))
			{
				OnColorSelect?.Invoke(color);
				PhotonManager.Instance.color.r = color.r;
				PhotonManager.Instance.color.g = color.g;
				PhotonManager.Instance.color.b = color.b;
				PhotonManager.Instance.color.a = color.a;
				group.SetActive(false);
			}
		}
	}

	public void ToggleColorPicker()
	{
		if (group.alpha == 0)
		{
			group.SetActive(true);
		}
		else if (group.alpha == 1)
		{
			group.SetActive(false);			
		}
	}
}
