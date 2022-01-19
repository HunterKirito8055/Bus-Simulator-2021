using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exampleDebug : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SingleTon<DebugPanel>.Exists)
        {
            DebugPanel.RegisterSection("Printing", 90, new DebugPanel.RenderDebugGUI(this.RenderDebugPanel));
        }
    }
    void RenderDebugPanel()
    {
        //GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        if (GUILayout.Button("Do Some Action Name", new GUILayoutOption[0]))
        {
            Debug.Log("Do some Action Here");
        }
        //  GUILayout.EndHorizontal();

    }
    // Update is called once per frame
    void Update()
    {

    }
}
