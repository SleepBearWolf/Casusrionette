// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace QuestsSystem.CustomEditor
{
    public class QuestLogicExist_EditorUI : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<QuestLogicExist_EditorUI, UxmlTraits> { }

        private QuestConfig currentQuestConfig;

        private Label questLogicScriptText;

        private Button openLogicButton;
        private Button deleteLogicFileButton;

        private string logicFilePath;

        /// <summary>
        /// Initialization of all components
        /// </summary>
        private void InitComponents()
        {
            questLogicScriptText = contentContainer.Q<Label>("QuestLogic-Script-Text");

            openLogicButton = contentContainer.Q<Button>("OpenLogic-Button");
            deleteLogicFileButton = contentContainer.Q<Button>("DeleteLogicFile-Button");
        }

        /// <summary>
        /// UI Initialization and Quest Assignment
        /// </summary>
        /// <param name="questConfig">Quest to assign</param>
        public void Init(QuestConfig questConfig)
        {
            InitComponents();

            currentQuestConfig = questConfig;
            logicFilePath = GetLogicScriptFile();

            openLogicButton.clicked += OpenLogicFile;
            deleteLogicFileButton.clicked += DeleteLogicFile;

            DrawLogicText();
        }

        /// <summary>
        /// Get logic script file path
        /// </summary>
        /// <returns>File path</returns>
        private string GetLogicScriptFile()
        {
            string logicFile = $"{currentQuestConfig.QuestName.ToPascal()}_QuestLogic";
            if (EditorUtilities.TryGetScriptFileInProject(logicFile, out string file))
            {
                return file;
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Open logic file in Script editor
        /// </summary>
        private void OpenLogicFile()
        {
            if (logicFilePath != string.Empty)
            {
                Process.Start(logicFilePath);
            }
        }

        /// <summary>
        /// Read all text from logic file script
        /// And draw in editor
        /// </summary>
        private void DrawLogicText()
        {
            if (logicFilePath != string.Empty)
            {
                string logic = File.ReadAllText(logicFilePath);
                questLogicScriptText.text = logic;
            }
        }

        /// <summary>
        /// Delete logic file from project
        /// </summary>
        private void DeleteLogicFile()
        {
            if (logicFilePath != string.Empty)
            {
                if (EditorUtility.DisplayDialog("Are u sure for DELETE?", "Logic file will be deleted permanently", "Delete", "Don't do this"))
                {
                    File.Delete(logicFilePath);
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
