#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {

    bool debugging;

    override public void OnInspectorGUI() {
        DrawDefaultInspector();

        GameManager myScript = (GameManager)target;

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