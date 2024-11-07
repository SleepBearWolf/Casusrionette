// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using UnityEditor;
using UnityEngine;

namespace QuestsSystem.CustomEditor
{
    /// <summary>
    /// Little custom inspector for QuestConfig
    /// Showing logic file status
    /// </summary>
    [UnityEditor.CustomEditor(typeof(QuestConfig))]
    public class QuestConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            QuestConfig config = (QuestConfig)target;

            if (config.LogicIsNull())
            {
                GUIStyle style = new GUIStyle(EditorStyles.textField);
                style.normal.textColor = Color.red;

                GUILayout.Label("LOGIC IS NULL", style);
            }
            else
            {
                GUIStyle style = new GUIStyle(EditorStyles.textField);
                style.normal.textColor = Color.green;

                GUILayout.Label("LOGIC IS FINE", style);
            }
        }
    }
}