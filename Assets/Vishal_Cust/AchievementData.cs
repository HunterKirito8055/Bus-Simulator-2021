using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Achievement Data", menuName = "Create New Achievement Data")]
public class AchievementData : ScriptableObject
{
    [SerializeField]
    public Achievement[] achievements;
}


[System.Serializable]
public class Achievement
{
    public Sprite icon;
    public string display;
    public string description;
    public string ID;
    public int current;
    public int goal;
    public bool unlocked;
    public bool hidden; //optional hidden attribute
}