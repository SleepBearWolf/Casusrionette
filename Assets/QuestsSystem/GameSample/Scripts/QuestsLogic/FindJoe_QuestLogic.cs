// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using QuestGameSample;
using UnityEngine;

namespace QuestsSystem
{
    public class FindJoe_QuestLogic : QuestLogic
    {
        public override QuestsNames QuestName => QuestsNames.FindJoe;

        public override string QuestTastText => "Find Joe";

        public override void OnAccept()
        {
            GameObject.FindObjectOfType<Joe_NPC>(true).gameObject.SetActive(true);
        }

        public override void Logic()
        {
            //The logic that works all the time while the quest is active
        }

        public override void Progress()
        {
            //Called when there is logic for progress (Call from any other entity)
        }

        public override void OnComplete()
        {
            QuestsManager.Instance.AddQuest(QuestsNames.TalkToJoe);
        }
    }
}