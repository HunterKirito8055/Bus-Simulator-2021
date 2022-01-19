using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform MainLevelParent;
    List<Transform> allLevelParents;
    [SerializeField] internal List<LevelInfo> levels = new List<LevelInfo>();
    [SerializeField] internal LevelInfo currentLevel;
    public LevelInfo CurrentLevel
    {
        get
        {
            return currentLevel;
        }
    }
    public int TotalLevels
    {
        get
        {
            return levels.Count;
        }
    }

    public int CurrentLevelNum
    {
        get
        {
            return GameManager.BusSimulation.CurrentLevelNum;
        }
    }
    private void Awake()
    {
        allLevelParents = new List<Transform>();
        for (int i = 0; i < MainLevelParent.childCount; i++)
        {
            allLevelParents.Add(MainLevelParent.GetChild(i));
        }
        for (int i = 0; i < allLevelParents.Count; i++)
        {
            LevelInfo levelInfo = new LevelInfo();

            levelInfo.SpawnPoint = allLevelParents[i].GetChild(0).transform;
            levelInfo.pickUp = allLevelParents[i].GetChild(1).GetComponent<PickDropArea>(); //  current level pickup is previous level's dropoff
            levelInfo.pickUp.busStopMode = BusStopMode.PICKUP;
            levelInfo.dropOff = allLevelParents[i].GetChild(2).GetComponent<PickDropArea>();
            levelInfo.dropOff.busStopMode = BusStopMode.DROPOFF;
            levelInfo.levelNum = i + 1;

            levels[i].levelNum = levelInfo.levelNum;
            levels[i].pickUp = levelInfo.pickUp;
            levels[i].dropOff = levelInfo.dropOff;
            levels[i].SpawnPoint = levelInfo.SpawnPoint;
            levels[i].levelName = "LEVEL " + levels[i].levelNum;
        }
        totalLevels = TotalLevels;
    }
    static int totalLevels;
    public static int GetTotalLevels()
    {
        return totalLevels;
    }

    public LevelInfo GetLevelInfo(int i)
    {
        return levels[i];
    }
    public void SetCurrentLevel(Transform t)
    {
        currentLevel = levels[CurrentLevelNum];
        t.position = currentLevel.SpawnPoint.position;
        t.rotation = currentLevel.SpawnPoint.rotation;
        DisableOtherLevels();
    }
    public void SetToFreeMode(Transform t)
    {
        t.position = levels[0].SpawnPoint.position;
        t.rotation = levels[0].SpawnPoint.rotation;
        DisableOtherLevels();
    }
    public void DisableOtherLevels()
    {
        for (int i = 0; i < allLevelParents.Count; i++)
        {
            allLevelParents[i].gameObject.SetActive(false);
        }
        if (!GameManager.UiManager.freeMode)
            allLevelParents[CurrentLevelNum].gameObject.SetActive(true);
    }

}//class

[System.Serializable]
public class LevelInfo
{
    public string levelName;
    public int levelNum;
    public Transform SpawnPoint;
    public PickDropArea pickUp;
    public PickDropArea dropOff;
    public int maxTimeBonus;
    public int maxTime;
}
