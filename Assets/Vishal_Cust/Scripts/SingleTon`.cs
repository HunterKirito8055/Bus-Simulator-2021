using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
    public static bool LazyCreate
    {
        get
        {
            return SingleTon<T>.g_bLazyCreate;
        }
        set
        {
            SingleTon<T>.g_bLazyCreate = value;
        }
    }

    public static T Create()
    {
        if (SingleTon<T>.g_instance == null)
        {
            SingleTon<T>.g_bLazyCreate = true;
        }
        PropertyInfo property = typeof(T).GetProperty("Get");
        if (property != null)
        {
            return property.GetValue(null, null) as T;
        }
        return SingleTon<T>.Get;
    }

    public static void Destroy()
    {
        if (SingleTon<T>.g_instance != null)
        {
            UnityEngine.Object.Destroy(SingleTon<T>.g_instance.gameObject);
            SingleTon<T>.g_instance = (T)((object)null);
        }
        SingleTon<T>.g_bLazyCreate = false;
    }

    public static bool Exists
    {
        get
        {
            return SingleTon<T>.g_instance != null;
        }
    }

    public static T Get
    {
        get
        {
            if (SingleTon<T>.g_instance == null)
            {
                SingleTon<T>.g_instance = (UnityEngine.Object.FindObjectOfType(typeof(T)) as T);
                if (SingleTon<T>.g_instance != null)
                {
                    SingleTon<T>.g_bLazyCreate = false;
                }
                else if (SingleTon<T>.g_bLazyCreate)
                {
                    SingleTon<T>.g_bLazyCreate = false;
                    string text = typeof(T).ToString();
                    GameObject gObject = null;

                    if (gObject == null)
                    {
                        gObject = (GameObject)Resources.Load(text + "Android", typeof(GameObject));
                    }

                    if (gObject == null)
                    {
                        gObject = (GameObject)Resources.Load(text, typeof(GameObject));
                    }
                    if (gObject != null)
                    {
                        GameObject gObject2 = UnityEngine.Object.Instantiate(gObject, Vector3.zero, Quaternion.identity) as GameObject;
                        SingleTon<T>.g_instance = gObject2.GetComponent<T>();
                    }
                    if (SingleTon<T>.g_instance == null)
                    {
                        SingleTon<T>.g_instance = new GameObject(text).AddComponent<T>();
                    }
                }
                if (SingleTon<T>.g_instance != null)
                {
                    SingleTon<T>.g_instance.gameObject.name = typeof(T).ToString();
                    UnityEngine.Object.DontDestroyOnLoad(SingleTon<T>.g_instance.gameObject);
                    UnityEngine.Object.DontDestroyOnLoad(SingleTon<T>.g_instance);
                }
            }
            return SingleTon<T>.g_instance;
        }
    }

    public void SetInstance()
    {
        if (SingleTon<T>.g_instance == null)
        {
            SingleTon<T>.g_instance = (T)((object)this);
            SingleTon<T>.g_instance.gameObject.name = typeof(T).ToString();
            UnityEngine.Object.DontDestroyOnLoad(SingleTon<T>.g_instance.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(SingleTon<T>.g_instance);
        }
    }

    private static T g_instance;

    private static bool g_bLazyCreate = true;
}
