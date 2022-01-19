using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public EUI_Menus menuName;
    public void Show()
    {
        switch (menuName)
        {
            case EUI_Menus.none:
                break;
            case EUI_Menus.mainMenu:
                break;
            case EUI_Menus.vehicleSelectionMenu:
                break;
            case EUI_Menus.storeMenu:
                break;
            case EUI_Menus.settingsMenu:
                break;
            case EUI_Menus.careerModeMenu:
                break;
            case EUI_Menus.levelSelectionMenu:
                break;
            case EUI_Menus.gameOver:
                break;
            case EUI_Menus.levelComplete:
                break;
            case EUI_Menus.pauseMenu:
                break;
            case EUI_Menus.gameTaskMenu:
                GameManager.AchievementManager.PopulateAchievementList();
                break;
            case EUI_Menus.areYouSure:
                break;
            default:
                break;
        }
        this.gameObject.SetActive(true);
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
