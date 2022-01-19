using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    public BusSimulation busSumulation;
    public LevelManager levelManager;
    public PassengerManager passengerManager;
    public UiManager uiManager;
    public Notification notification;
    public AchievementManager achievementManager;
    public RCC_CarSelectionExample carSelectionExample;

    static public BusSimulation BusSimulation
    {
        get
        {
            return SingleTon<GameManager>.Get.busSumulation;
        }
    }

    static public PassengerManager PassengerManager
    {
        get
        {
            return SingleTon<GameManager>.Get.passengerManager;
        }
    }
    static public UiManager UiManager
    {
        get
        {
            return SingleTon<GameManager>.Get.uiManager;
        }
    }

    static public Notification Notification
    {
        get
        {
            return SingleTon<GameManager>.Get.notification;
        }
    }
    static public LevelManager LevelManager
    {
        get
        {
            return SingleTon<GameManager>.Get.levelManager;
        }
    }
    static public AchievementManager AchievementManager
    {
        get
        {
            return SingleTon<GameManager>.Get.achievementManager;
        }
    }
    static public RCC_CarSelectionExample CarSelectionExample
    {
        get
        {
            return SingleTon<GameManager>.Get.carSelectionExample;
        }
    }

    private void Start()
    {
        UnityEngine.Object.DontDestroyOnLoad(this);
    }
}
