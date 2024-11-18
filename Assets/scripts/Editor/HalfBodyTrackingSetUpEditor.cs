using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HalfBodyTrackingCharacterSetUp))]
public class HalfBodyTrackingSetUpEditor : Editor
{
    public HalfBodyTrackingCharacterSetUp script { get { return target as HalfBodyTrackingCharacterSetUp; } }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Set Up Character", GUILayout.Height(30)))
        {
            script.SetUpCharacter();
        }

        GUILayout.Space(10);
    }

}
