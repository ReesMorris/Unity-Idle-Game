#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IdleGameManager))]
public class IdleGameManagerEditor : Editor {

    bool debugging;

    override public void OnInspectorGUI() {
        DrawDefaultInspector();

        IdleGameManager myScript = (IdleGameManager)target;

        // Debugging title and toggle
        GUILayout.Label("Debugging", EditorStyles.boldLabel);
        debugging = EditorGUILayout.Toggle("Show Debug Menu", debugging);

        if (debugging) {
            if (GUILayout.Button("Reset Saved Data")) {
                myScript.Reset();
            }
        }
    }
}
#endif