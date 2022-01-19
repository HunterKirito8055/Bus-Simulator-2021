//////////////////////////

/*

Debug Panel Usage Instructions..

In a class you want the method to show in panel, use 
 
///
*/
#region Instructions
/*if (SingleTon<DebugPanel>.Exists)
{
DebugPanel.RegisterSection("Unlock All Level", 90, new DebugPanel.RenderDebugGUI(this.RenderDebugPanel));
}
RenderDebugPanel <== is a method in that class

for example
public void RenderDebugPanel()
{

    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    if (GUILayout.Button("Unlock All Levels", new GUILayoutOption[0]))
    {
        foreach (var item in levelSelectionMenu.levelBtnScripts)
        {
            item.BtnLock(true);
        }
    }
    GUILayout.EndHorizontal();

}

*/

#region singleton
/*
 ///
//Singleton Class Scripts..
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

//////
/////////////

 
 */
#endregion
#endregion








using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("Hutch/Debug/Debug Panel")]
public class DebugPanel : SingleTon<DebugPanel>
{
    public static event DebugPanel.OnDebugVisiblityChanged VisibilityChanged;

    public static GUIStyle TopOpenButtonStyle
    {
        get
        {
            return SingleTon<DebugPanel>.Get.m_topOpenButtonStyle;
        }
    }

    public static GUIStyle OpenButtonStyle
    {
        get
        {
            return SingleTon<DebugPanel>.Get.m_openButtonStyle;
        }
    }

    public static GUIStyle CloseButtonStyle
    {
        get
        {
            return SingleTon<DebugPanel>.Get.m_closeButtonStyle;
        }
    }

    public static GUIStyle BackgroundStyle
    {
        get
        {
            return SingleTon<DebugPanel>.Get.m_backgroundStyle;
        }
    }

    public static GUIStyle TitleStyle
    {
        get
        {
            return SingleTon<DebugPanel>.Get.m_titleStyle;
        }
    }

    private void Awake()
    {
        SingleTon<DebugPanel>.Create();
        // base.enabled = false;  Vishal 08-07-2021
    }

    private void Start()
    {
        if (this.m_debugSkin != null)
        {
            this.m_topOpenButtonStyle = this.m_debugSkin.customStyles[0];
            this.m_openButtonStyle = this.m_debugSkin.customStyles[1];
            this.m_closeButtonStyle = this.m_debugSkin.customStyles[2];
            this.m_backgroundStyle = this.m_debugSkin.customStyles[3];
            this.m_titleStyle = this.m_debugSkin.customStyles[4];
        }
        this.m_eventSystem = EventSystem.current;
        this.LoadSavedScreenWidth();
    }

    public static void RegisterSection(string sName, int nPriority, DebugPanel.RenderDebugGUI callback)
    {
        DebugPanel.DebugSection item = new DebugPanel.DebugSection
        {
            priority = nPriority,
            sName = sName,
            callback = callback,
            bOpened = false
        };
        if (SingleTon<DebugPanel>.Exists)
        {
            if (SingleTon<DebugPanel>.Get.m_debugSections.Contains(item))
            {
                return;
            }
            SingleTon<DebugPanel>.Get.m_debugSections.Add(item);
            switch (SingleTon<DebugPanel>.Get.m_sortOrder)
            {
                case DebugPanel.SortOrder.Priority:
                    SingleTon<DebugPanel>.Get.m_debugSections = (from x in SingleTon<DebugPanel>.Get.m_debugSections
                                                                 orderby x.priority, x.sName
                                                                 select x).ToList<DebugPanel.DebugSection>();
                    break;
                case DebugPanel.SortOrder.Title:
                    SingleTon<DebugPanel>.Get.m_debugSections = (from x in SingleTon<DebugPanel>.Get.m_debugSections
                                                                 orderby x.sName
                                                                 select x).ToList<DebugPanel.DebugSection>();
                    break;
            }
        }
    }

    public static void UnregisterSection(string sName)
    {
        if (SingleTon<DebugPanel>.Exists)
        {
            DebugPanel.DebugSection debugSection = (from x in SingleTon<DebugPanel>.Get.m_debugSections
                                                    where x.sName == sName
                                                    select x).FirstOrDefault<DebugPanel.DebugSection>();
            if (debugSection != null)
            {
                int index = SingleTon<DebugPanel>.Get.m_debugSections.IndexOf(debugSection);
                SingleTon<DebugPanel>.Get.m_debugSections.RemoveAt(index);
            }
        }
    }

    public static void Close()
    {
        if (SingleTon<DebugPanel>.Get.m_isVisible)
        {
            SingleTon<DebugPanel>.Get.m_isVisible = false;
            if (SingleTon<DebugPanel>.Get.m_eventSystem != null)
            {
                SingleTon<DebugPanel>.Get.m_eventSystem.gameObject.SetActive(true);
            }
            if (DebugPanel.VisibilityChanged != null)
            {
                DebugPanel.VisibilityChanged(SingleTon<DebugPanel>.Get.m_isVisible);
            }
        }
    }

    private void DisplayDebugPanel(float screenWidth, float screenHeight)
    {
        if (this.m_transitionScale != 0f)
        {
            Rect screenRect = new Rect(0f, 0f, screenWidth, Mathf.Max(screenHeight * this.m_transitionScale, 1f));
            GUILayout.BeginArea(screenRect, this.m_backgroundStyle);
            GUILayout.Space(this.m_buttonSize.y * 1.25f);
            this.m_scrollPos = GUILayout.BeginScrollView(this.m_scrollPos, new GUILayoutOption[0]);
            foreach (DebugPanel.DebugSection debugSection in this.m_debugSections)
            {
                if (debugSection.bOpened)
                {
                    GUILayout.BeginVertical(this.m_closeButtonStyle, new GUILayoutOption[]
                    {
                        GUILayout.ExpandWidth(true)
                    });
                    if (GUILayout.Button(debugSection.sName, this.m_titleStyle, new GUILayoutOption[0]))
                    {
                        debugSection.bOpened = false;
                    }
                    try
                    {
                        debugSection.callback();
                    }
                    catch (Exception ex)
                    {
                        GUILayout.Label("Error in the code! See console!", new GUILayoutOption[0]);
                        GUILayout.Label(ex.Message, new GUILayoutOption[0]);
                        UnityEngine.Debug.LogException(ex);
                    }
                    GUILayout.EndVertical();
                }
                else if (GUILayout.Button(debugSection.sName, this.m_openButtonStyle, new GUILayoutOption[]
                {
                    GUILayout.ExpandWidth(false)
                }))
                {
                    debugSection.bOpened = true;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }

    //private void OnGUI()
    //{
    //    float num = (float)Screen.width / (float)this.m_screenWidth;
    //    bool flag = this.m_scaleDebugPanel && (!this.m_onlyScaleForBiggerScreens || num > 1f);
    //    Matrix4x4 matrix = GUI.matrix;
    //    float num2 = (float)Screen.width;
    //    float screenHeight = (float)Screen.height;
    //    if (flag)
    //    {
    //        Matrix4x4 rhs = default(Matrix4x4);
    //        rhs.SetTRS(Vector3.zero, Quaternion.identity, Vector3.one * num);
    //        GUI.matrix *= rhs;
    //        num2 = (float)this.m_screenWidth;
    //        screenHeight = (float)Screen.height / num;
    //    }
    //    GUI.skin = this.m_debugSkin;
    //    this.DisplayDebugPanel(num2, screenHeight);
    //    Rect position = new Rect(num2 * this.m_xPosition - this.m_buttonSize.x / 2f, 0f, this.m_buttonSize.x, this.m_buttonSize.y);
    //    GUIStyle guistyle = (!this.m_shouldDebugButtonBeVisible) ? GUIStyle.none : this.m_topOpenButtonStyle;
    //    string text = (!this.m_shouldDebugButtonBeVisible) ? string.Empty : "Open Debug";
    //    if (this.m_isVisible)
    //    {
    //        if (GUI.Button(position, "Close Debug", this.m_closeButtonStyle))
    //        {
    //            this.m_isVisible = false;
    //            if (this.m_eventSystem != null)
    //            {
    //                this.m_eventSystem.gameObject.SetActive(true);
    //            }
    //            if (DebugPanel.VisibilityChanged != null)
    //            {
    //                DebugPanel.VisibilityChanged(this.m_isVisible);
    //            }
    //        }
    //        Rect position2 = new Rect(0f, 0f, this.m_buttonSize.x, this.m_buttonSize.y);
    //        Rect position3 = new Rect(num2 - this.m_buttonSize.x, 0f, this.m_buttonSize.x, this.m_buttonSize.y);
    //        if (GUI.Button(position2, "Chunkier"))
    //        {
    //            this.m_screenWidth = this.m_screenWidth * 3 / 4;
    //            this.SaveSavedScreemWidth();
    //        }
    //        if (GUI.Button(position3, "Slimmer"))
    //        {
    //            this.m_screenWidth = this.m_screenWidth * 4 / 3;
    //            this.SaveSavedScreemWidth();
    //        }
    //    }
    //    else if (guistyle != null && GUI.Button(position, text, guistyle))
    //    {
    //        this.m_isVisible = true;
    //        if (this.m_eventSystem != null)
    //        {
    //            this.m_eventSystem.gameObject.SetActive(false);
    //        }
    //        if (DebugPanel.VisibilityChanged != null)
    //        {
    //            DebugPanel.VisibilityChanged(this.m_isVisible);
    //        }
    //    }
    //    if (Event.current.type == EventType.Repaint)
    //    {
    //        float num3 = (!this.m_isVisible) ? -1f : 1f;
    //        this.m_transitionScale = Mathf.Clamp01(this.m_transitionScale + (float)(AudioSettings.dspTime - this.m_oldDspTime) * this.m_transitionSpeed * num3);
    //        this.m_oldDspTime = AudioSettings.dspTime;
    //    }
    //    GUI.matrix = matrix;
    //}

    //public static bool DebugIntControls(string label, ref int value, int min = -2147483648, int max = 2147483647, int increment = 1)
    //{
    //    int num = value;
    //    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //    GUILayout.Label(label, new GUILayoutOption[0]);
    //    if (GUILayout.Button("  -  ", new GUILayoutOption[0]))
    //    {
    //        value -= increment;
    //    }
    //    GUILayout.Label(value.ToString(), new GUILayoutOption[0]);
    //    if (GUILayout.Button("  +  ", new GUILayoutOption[0]))
    //    {
    //        value += increment;
    //    }
    //    value = Mathf.Clamp(value, min, max);
    //    GUILayout.EndHorizontal();
    //    return num != value;
    //}

    public static bool IsOpen
    {
        get
        {
            return SingleTon<DebugPanel>.Exists && SingleTon<DebugPanel>.Get.m_isVisible;
        }
    }

    private void LoadSavedScreenWidth()
    {
    }

    private void SaveSavedScreemWidth()
    {
    }

    public static readonly string s_savedScreenWidthKey = "DebugPanelSavedScreenWidth";

    [SerializeField]
    private bool m_shouldDebugButtonBeVisible = true;

    [SerializeField]
    private Vector2 m_buttonSize = new Vector2(200f, 50f);

    [SerializeField]
    private float m_xPosition = 0.5f;

    [SerializeField]
    private GUISkin m_debugSkin;

    [SerializeField]
    private float m_transitionSpeed = 3f;

    [SerializeField]
    private bool m_scaleDebugPanel;

    [SerializeField]
    private int m_screenWidth = 1024;

    [SerializeField]
    private bool m_onlyScaleForBiggerScreens = true;

    [SerializeField]
    private DebugPanel.SortOrder m_sortOrder;
    [SerializeField]
    private List<DebugPanel.DebugSection> m_debugSections = new List<DebugPanel.DebugSection>();

    private bool m_isVisible;

    private Vector2 m_scrollPos = Vector2.zero;

    private float m_transitionScale;

    private double m_oldDspTime;

    private EventSystem m_eventSystem;

    private GUIStyle m_topOpenButtonStyle;

    private GUIStyle m_openButtonStyle;

    private GUIStyle m_closeButtonStyle;

    private GUIStyle m_backgroundStyle;

    private GUIStyle m_titleStyle;

    public enum SortOrder
    {
        Priority,
        InitOrder,
        Title
    }
    [Serializable]
    private class DebugSection
    {
        public int priority;

        public string sName;

        public DebugPanel.RenderDebugGUI callback;

        public bool bOpened;
    }

    public delegate void RenderDebugGUI();

    public delegate void OnDebugVisiblityChanged(bool isDebugVisible);
}
