// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuestsSystem
{
    public class QuestsUI : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<QuestsUI, UxmlTraits> { }

        private static QuestsUI instance;

        /// <summary>
        /// When called in case of not initialized components, 
        /// it initialize them
        /// </summary>
        public static QuestsUI Instance
        {
            get
            {
                if (instance == null)
                    return null;

                if (!instance.initialised)
                {
                    instance.InitComponents(); //Isitialize check
                }
                return instance;
            }
            private set { instance = value; }
        }

        public bool initialised;

        private VisualElement questsListPanel; //Visual element in UI document
        private List<QuestUI> instantiatedQuestsUI = new List<QuestUI>();

        public QuestsUI()
        {
            Instance = this;
        }

        /// <summary>
        /// Initialization of all components
        /// </summary>
        public void InitComponents()
        {
            questsListPanel = contentContainer.Q<VisualElement>("QuestsList-Panel");
            initialised = true;
        }

        /// <summary>
        /// Drawing all active quests
        /// </summary>
        public void DrawQuests()
        {
            questsListPanel.Clear(); //Clear ui list element

            foreach (QuestConfig questConfig in QuestsManager.Instance.ActiveQuests)
            {
                //Load visual tree from asset file
                VisualTreeAsset questVisualTree = Resources.Load<VisualTreeAsset>("QuestsSystem/UI/QuestUI");
                VisualElement questFromUXML = questVisualTree.Instantiate();

                questFromUXML.Q<QuestUI>("QuestUI").Init(questConfig); //Init ui quest element

                questsListPanel.Add(questFromUXML); //Add to ui list
            }

            instantiatedQuestsUI = questsListPanel.Query<QuestUI>().ToList(); //Get and convert all elements to list
        }

        /// <summary>
        /// Update one specific quest without redrawing the entire sheet
        /// </summary>
        /// <param name="questConfig">Quest to redraw</param>
        public void UpdateQuest(QuestConfig questConfig)
        {
            foreach (QuestUI questUI in instantiatedQuestsUI)
            {
                if (questUI.currentQuest == questConfig)
                {
                    questUI.DrawQuest();
                }
            }
        }
    }
}