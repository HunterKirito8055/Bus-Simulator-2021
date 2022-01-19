using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickBtn : MonoBehaviour
{
    public Text clickDisplay;
    public int clicks;

    public void Click()
    {
        clicks++;
        clickDisplay.text = clicks + " CLICKS";

        GameManager.AchievementManager.AddAchievementProgress("TUT_ACH_CLICK", 1);
        GameManager.AchievementManager.AddAchievementProgress("TUT_ACH_CLICKY", 1);
        GameManager.AchievementManager.AddAchievementProgress("TUT_ACH_GLOO", 1);
    }
}
