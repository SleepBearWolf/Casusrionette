// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using System;
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace QuestsSystem.CustomEditor
{
    public class QuestItem_EditorUI : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<QuestItem_EditorUI, UxmlTraits> { }

        public QuestConfig currentQuestConfig { get; private set; }

        private Label questNameText;
        private Label questEnumNameText;

        private VisualElement questFileError;

        private Button deleteQuestButton;

        /// <summary>
        /// Initialization of all components
        /// </summary>
        private void InitComponents()
        {
            questNameText = contentContainer.Q<Label>("QuestName-Text");
            questEnumNameText = contentContainer.Q<Label>("QuestEnumName-Text");
            questFileError = contentContainer.Q<VisualElement>("QuestFileError");
            deleteQuestButton = contentContainer.Q<Button>("DeleteQuest-Button");
        }

        /// <summary>
        /// UI Initialization and Quest Assignment
        /// </summary>
        /// <param name="questConfig">Quest to assign</param>
        public void Init(QuestConfig questConfig)
        {
            InitComponents();

            currentQuestConfig = questConfig;

            questNameText.text = questConfig.QuestName;

            CheckEnumNameCorrect();
            CheckQuestFileCorrect();

            contentContainer.RegisterCallback((ClickEvent ce) =>
            {
                Quest_OnClick(); //QuestItem click event
            });

            deleteQuestButton.clicked += DeleteQuestButton_OnClick;
        }

        /// <summary>
        /// Check current quest config for enum name errors
        /// </summary>
        /// <param name="questConfig">Quest config for check</param>
        private void CheckEnumNameCorrect()
        {
            if (Enum.TryParse(currentQuestConfig.QuestName.ToPascal(), out QuestsNames result))
            {
                questEnumNameText.text = $"<color=yellow>Enum:</color> {result.ToString()}"; //Draw quest enum name
            }
            else
                questEnumNameText.text = $"<color=red>Can't find it in QuestsNames</color>";
        }

        /// <summary>
        /// Check quest file for errors
        /// </summary>
        private void CheckQuestFileCorrect()
        {
            if (currentQuestConfig.name != currentQuestConfig.QuestName.ToPascal())
            {
                questFileError.style.display = DisplayStyle.Flex; //Activate error visual element

                Label questFileNameValidText = contentContainer.Q<Label>("QuestFileName-Valid");

                //Draw error text
                string title = $"Quest file name doesn't synced \n";
                string info = $"File: {currentQuestConfig.name} | Quest: {currentQuestConfig.QuestName.ToPascal()}";
                questEnumNameText.text = title + info;

                //Adding event for fix
                contentContainer.Q<Button>("QuestFileFix-Button").clicked += () =>
                {
                    QuestFileFix_OnClick(currentQuestConfig);
                };
            }
            else
                questFileError.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Renaming target quest config file
        /// </summary>
        /// <param name="questConfig">Target quest config</param>
        private void QuestFileFix_OnClick(QuestConfig questConfig)
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(questConfig), questConfig.QuestName.ToPascal()); //Renaming asset file
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(); //Refresh project
            QuestsEditorWindow.Instance.RedrawGUI(); //Redraw quests editor
        }

        /// <summary>
        /// Select this quest item when clicked
        /// </summary>
        private void Quest_OnClick()
        {
            QuestsEditorWindow.Instance.SelectQuest(this);
        }

        /// <summary>
        /// Delete quest config from project
        /// </summary>
        private void DeleteQuestButton_OnClick()
        {
            string assetPath = AssetDatabase.GetAssetPath(currentQuestConfig); //Getting asset path in project

            //Display dialog for confirm
            if (EditorUtility.DisplayDialog("Are u sure for DELETE?", "Config and Logic files will be deleted permanently", "Delete", "Don't do this"))
            {
                AssetDatabase.DeleteAsset(assetPath); //Delete asset file

                //If config has logic file
                //Try to find it and also delete
                if (EditorUtilities.TryGetScriptFileInProject($"{currentQuestConfig.QuestName}_QuestLogic", out string filePath))
                {
                    File.Delete(filePath);
                }

                AssetDatabase.Refresh(); //Refresh project
                QuestsEditorWindow.Instance.RedrawGUI(); //Redraw quests editor
            }
        }

        /// <summary>
        /// Add uss selector to this item
        /// </summary>
        public void Select()
        {
            contentContainer.AddToClassList("selected");
        }

        /// <summary>
        /// Remove uss selector from this item
        /// </summary>
        public void Deselect()
        {
            contentContainer.RemoveFromClassList("selected");
        }
    }
}