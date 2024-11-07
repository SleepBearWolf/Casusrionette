// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QuestsSystem
{
    public class QuestsManager : MonoBehaviour
    {
        public static QuestsManager Instance { get; private set; }

        [SerializeField] private List<QuestConfig> activeQuests = new List<QuestConfig>(); //List of active quests

        /// <summary>
        /// Field for getting a list of active quests
        /// Use case: Drawing in UI
        /// </summary>
        public List<QuestConfig> ActiveQuests => activeQuests;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            ActiveQuestsTick();
        }

        /// <summary>
        /// Calling the quest logic method every frame
        /// </summary>
        private void ActiveQuestsTick()
        {
            for (int i = 0; i < activeQuests.Count; i++)
            {
                //"?" Nullable check
                activeQuests[i].QuestLogic?.Logic();
            }
        }

        /// <summary>
        /// Checking the quest for availability
        /// </summary>
        /// <param name="questName">Quest for check</param>
        /// <returns></returns>
        public bool IsQuestActive(QuestsNames questName)
        {
            foreach (QuestConfig questConfig in activeQuests)
            {
                if (questConfig.QuestName.ToPascal() == questName.ToString())
                {
                    return true;
                }
            }

            //If doesn't exist
            return false;
        }

        /// <summary>
        /// Adding a quest to the active list
        /// </summary>
        /// <param name="questName">Quest to add</param>
        public void AddQuest(QuestsNames questName)
        {
            QuestConfig quest = QuestConfig.GetConfig(questName);

            if (quest != null)
            {
                //Non duplicate check
                if (!activeQuests.Contains(quest))
                {
                    quest.QuestLogic?.OnAccept();

                    activeQuests.Add(quest);

                    QuestsUI.Instance?.DrawQuests(); //Game ui redraw quests list
                }
            }
        }

        /// <summary>
        /// Updating progress in a specific quest
        /// </summary>
        /// <param name="questName">Update Quest</param>
        public void UpdateQuestProgress(QuestsNames questName)
        {
            QuestConfig targetQuestConfig = QuestConfig.GetConfig(questName);

            if (targetQuestConfig == null)
                return;

            if (activeQuests.Contains(targetQuestConfig))
            {
                foreach (QuestConfig quest in activeQuests.ToList())
                {
                    if (quest == targetQuestConfig)
                    {
                        quest.QuestLogic?.Progress();
                        QuestsUI.Instance?.UpdateQuest(targetQuestConfig);
                    }
                }
            }
        }

        /// <summary>
        /// Removing a quest from the active list
        /// </summary>
        /// <param name="questName">Quest to remove</param>
        /// <param name="completed">Remove type</param>
        public void RemoveQuest(QuestsNames questName, bool completed)
        {
            QuestConfig quest = QuestConfig.GetConfig(questName);

            if (quest != null)
            {
                if (activeQuests.Contains(quest))
                {
                    if (completed)
                        quest.QuestLogic?.OnComplete();

                    activeQuests.Remove(quest);
                    QuestsUI.Instance?.DrawQuests();
                }
            }
        }
    }
}