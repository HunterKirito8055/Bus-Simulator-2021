using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class PlatformDataManager : MonoBehaviour
{
    public GameObject platformPrefab;

    public List<Transform> platforms;

    public PlatformData platformData;
    public string path;
    private void OnValidate()
    {
        platforms = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            platforms.Add(transform.GetChild(i));
        }
    }
    public void Start()
    {
        path = SaveSystem.path;
        platformPrefab = Resources.Load<GameObject>("Platform");
        //#if !UNITY_EDITOR && PLATFORM_ANDROID
        if (File.Exists(path))
        {
            LoadData();
        }
        //#endif
    }
    public void SaveData()
    {
        platformData = new PlatformData(this);
        SaveSystem.SavePositions(this, platformData);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            LoadData();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            SaveData();
        }
    }
    public void LoadData()
    {
        platforms = new List<Transform>();
        platformData = SaveSystem.LoadPositions();
        for (int i = 0; i < platformData.positions.Length; i++)
        {
            GameObject newPlat = Instantiate(platformPrefab, this.transform);
            newPlat.transform.localScale = platformData.scales[i];
            newPlat.transform.eulerAngles = platformData.rotations[i];
            newPlat.transform.position = platformData.positions[i];
            if (!newPlat.GetComponent<BoxCollider>())
                newPlat.AddComponent<BoxCollider>();
            platforms.Add(newPlat.transform);
        }
    }// platformData.ToVector(platformData.positions, i)
     //, Quaternion.Euler(platformData.ToVector(platformData.rotations, i))
}//class


public static class SaveSystem
{
    public static string path = Application.dataPath + "/Vishal_Cust/Resources/platformData.txt";

    public static void SavePositions(PlatformDataManager dataManager, PlatformData data)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PlatformData));
        FileStream stream = new FileStream(path, FileMode.Create);
        serializer.Serialize(stream, data);

        stream.Close();
    }
    public static PlatformData LoadPositions()
    {
        if (File.Exists(path))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PlatformData));
            FileStream strea = new FileStream(path, FileMode.Open);

            PlatformData data = serializer.Deserialize(strea) as PlatformData;
            strea.Close();
            return data;
        }
        else
        {
            Debug.LogError("Saved File not found at ==> " + path);
            return null;
        }

    }

}//saveSystem


[System.Serializable]
public class PlatformData
{

    [SerializeField]
    public Vector3[] positions;
    [SerializeField]
    public Vector3[] scales;
    [SerializeField]
    public Vector3[] rotations;
    public PlatformData()
    {

    }
    public PlatformData(PlatformDataManager dataManager)
    {
        rotations = new Vector3[dataManager.platforms.Count];
        positions = new Vector3[dataManager.platforms.Count];
        scales = new Vector3[dataManager.platforms.Count];

        for (int i = 0; i < dataManager.platforms.Count; i++)
        {
            Vector3 pos = dataManager.platforms[i].position;
            Vector3 sca = dataManager.platforms[i].localScale;
            Vector3 rot = dataManager.platforms[i].eulerAngles;
            Debug.LogFormat("{0},{1},{2} || {3},{4},{5} || {6},{7},{8} || {9}", pos.x, pos.y, pos.z, sca.x, sca.y, sca.z, rot.x, rot.y, rot.z, i);
            positions[i] = pos;
            scales[i] = sca;
            rotations[i] = rot;

        }
    }

    //public Vector3 ToVector(float[][] v, int i)
    //{
    //    Debug.LogFormat("{0},{1},{2} <== {3}", v[i][0], v[i][1], v[i][2], i);
    //    return new Vector3(v[i][0], v[i][1], v[i][2]);
    //}
    public struct Vector3D
    {
        public float x, y, z;
        public Vector3D(Vector3 vec)
        {
            x = vec.x; y = vec.y; z = vec.z;
        }
    }

}//platform Data