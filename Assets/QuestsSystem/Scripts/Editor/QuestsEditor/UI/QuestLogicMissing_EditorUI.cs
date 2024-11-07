// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace QuestsSystem.CustomEditor
{
    public class QuestLogicMissing_EditorUI : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<QuestLogicMissing_EditorUI, UxmlTraits> { }

        private QuestConfig currentQuestConfig;
        private Button createLogicButton;

        private const string NEW_QUEST_LOGIC_PATH = "Assets/QuestsSystem/Scripts/QuestsSystem/QuestsLogic/";
        private const string QUEST_LOGIC_TEMPLATE_FILE_NAME = "QuestLogic_Template";

        /// <summary>
        /// UI Initialization and Quest Assignment
        /// </summary>
        /// <param name="questConfig">Quest to assign</param>
        public void Init(QuestConfig questConfig)
        {
            currentQuestConfig = questConfig;
            createLogicButton = contentContainer.Q<Button>("CreateLogic-Button");
            createLogicButton.clicked += CreateQuestLogicFile;
        }

        /// <summary>
        /// Create new logic file for current quest config
        /// </summary>
        private void CreateQuestLogicFile()
        {
            string fileName = $"{currentQuestConfig.QuestName.ToPascal()}_QuestLogic.cs";
            string filePath = NEW_QUEST_LOGIC_PATH + fileName;

            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(NEW_QUEST_LOGIC_PATH))
                {
                    Directory.CreateDirectory(NEW_QUEST_LOGIC_PATH);
                }

                //Open file
                using (StreamWriter outFile = new StreamWriter(filePath))
                {
                    string templateFile = EditorUtilities.GetFileInProject(QUEST_LOGIC_TEMPLATE_FILE_NAME, ".txt"); //Find logic template file in project
                    string templateText = File.ReadAllText(templateFile);

                    string replacedText = templateText.Replace("{QUEST_NAME}", currentQuestConfig.QuestName.ToPascal()); //Replace name to new

                    outFile.Write(replacedText); //Write to file
                }
            }

            AssetDatabase.Refresh();
        }
    }
}