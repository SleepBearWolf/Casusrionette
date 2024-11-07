// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using UnityEngine.UIElements;

namespace QuestsSystem
{
    public class QuestUI : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<QuestUI, UxmlTraits> { }

        public QuestConfig currentQuest { get; private set; }

        private Label questNameText;
        private Label questDescriptionText;
        private Label questTaskText;

        /// <summary>
        /// Initialization of all components
        /// </summary>
        private void InitComponents()
        {
            questNameText = contentContainer.Q<Label>("QuestName-Text");
            questDescriptionText = contentContainer.Q<Label>("QuestDescription-Text");
            questTaskText = contentContainer.Q<Label>("QuestTask-Text");
        }

        /// <summary>
        /// UI Initialization and Quest Assignment
        /// </summary>
        /// <param name="questConfig">Quest to assign</param>
        public void Init(QuestConfig questConfig)
        {
            InitComponents();

            currentQuest = questConfig;

            DrawQuest();
        }

        /// <summary>
        /// Drawing the quest
        /// </summary>
        public void DrawQuest()
        {
            questNameText.text = currentQuest.QuestName;
            questDescriptionText.text = currentQuest.QuestDescription;

            if (currentQuest.QuestLogic != null)
                questTaskText.text = currentQuest.QuestLogic.QuestTastText;
            else
                questTaskText.style.display = DisplayStyle.None;
        }

    }
}