using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

[Serializable]
public class ColorEvent : UnityEvent<Color> { }

//https://www.youtube.com/watch?v=rKhFYxUNL6A
public class ColorPicker : MonoBehaviour
{
	public ColorEvent OnColorPreview;
	public ColorEvent OnColorSelect;
	public GameObject Selector;
	RectTransform Rect;
	Texture2D ColorTexture;
	CanvasGroup group;

	private void Start()
	{
		Rect = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();

		ColorTexture = GetComponentInChildren<Image>().mainTexture as Texture2D;

		group = gameObject.GetComponentInChildren<CanvasGroup>();
	}

	private void Update()
	{
		if (group.activeInHierarchy() && RectTransformUtility.RectangleContainsScreenPoint(Rect, Input.mousePosition))
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
		group.Toggle();
	}
}
