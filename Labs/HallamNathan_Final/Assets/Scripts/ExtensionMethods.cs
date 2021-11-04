using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
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

    public static bool activeInHierarchy(this CanvasGroup CG)
    {
        return (CG.alpha == 1 ? true : false);
    }
}
