using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(PlatformDataManager))]
[CanEditMultipleObjects]
public class PlatformDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlatformDataManager pdm = (PlatformDataManager)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Road Data"))
        {
            pdm.SaveData();
        }
        if (GUILayout.Button("Load Road Data"))
        {
            pdm.LoadData();
        }
        GUILayout.EndHorizontal();
    }
}
