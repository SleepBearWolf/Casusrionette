// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using System;
using System.Reflection;
using UnityEngine;

namespace QuestsSystem
{
    [CreateAssetMenu(menuName = "Quests/Quest config", fileName = "Quest config")]
    public class QuestConfig : ScriptableObject
    {
        private const string CONFIGS_PATH = "Configs/Quests/"; //Path in resources folder to find all quests configs

        public string QuestName;
        public string QuestDescription;

        private QuestLogic questLogic;

        /// <summary>
        /// Using the logic of the quest without creating unnecessary references in the code
        /// Logic works completely independent of any of your components
        /// There is no need to create instances in your code
        /// </summary>
        public QuestLogic QuestLogic
        {
            get
            {
                if (questLogic == null)
                {
                    //Create an instance at the Assembly level
                    questLogic = (QuestLogic)Assembly.GetExecutingAssembly().CreateInstance($"QuestsSystem.{QuestName.ToPascal()}_QuestLogic");
                }

                return questLogic;
            }
        }

        public QuestConfig()
        {
            QuestName = "New quest";
            QuestDescription = "New quest decription";
        }

        /// <summary>
        /// Returns the desired config on request using enum
        /// </summary>
        /// <param name="questName">Enum of the required quest to find the config</param>
        /// <returns></returns>
        public static QuestConfig GetConfig(QuestsNames questName)
        {
            QuestConfig[] quests = Resources.LoadAll<QuestConfig>(CONFIGS_PATH); //Load all quests configs in the project

            foreach (QuestConfig quest in quests)
            {
                if (quest.QuestName.ToPascal() == questName.ToString())
                {
                    return quest;
                }
            }

            return null;

            //Can be used LinQ
            //return (QuestConfig)(from QuestConfig questConfig in quests where questConfig.QuestName.ToPascal() == questName.ToString() select questConfig);
        }

        /// <summary>
        /// Null check for quest logic
        /// </summary>
        /// <returns>If type of quest logic is not exist returns false</returns>
        public bool LogicIsNull()
        {
            return Type.GetType($"QuestsSystem.{QuestName.ToPascal()}_QuestLogic") == null;
        }
    }
}