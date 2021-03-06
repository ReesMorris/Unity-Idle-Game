﻿#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IdleMoneyManager))]
public class IdleMoneyManagerEditor : Editor {

    bool debugging;
    double giveMoney;
    double reduceMoney;

    override public void OnInspectorGUI() {
        DrawDefaultInspector();

        IdleMoneyManager myScript = (IdleMoneyManager)target;

        // Debugging title and toggle
        GUILayout.Label("Debugging", EditorStyles.boldLabel);
        debugging = EditorGUILayout.Toggle("Show Debug Menu", debugging);

        if (debugging) {
            // Give money layout
            GUILayout.BeginHorizontal();
            giveMoney = EditorGUILayout.DoubleField("Give Money", giveMoney);
            if (GUILayout.Button("Go")) {
                myScript.AddMoney(giveMoney);
            }
            GUILayout.EndHorizontal();

            // Reduce money layout
            GUILayout.BeginHorizontal();
            reduceMoney = EditorGUILayout.DoubleField("Take Money", reduceMoney);
            if (GUILayout.Button("Go")) {
                myScript.ReduceMoney(reduceMoney);
            }
            GUILayout.EndHorizontal();
        }
    }
}
#endif