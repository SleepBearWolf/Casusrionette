// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

namespace QuestsSystem
{
    public class WelcomeToTheVillage_QuestLogic : QuestLogic
    {
        public override QuestsNames QuestName => QuestsNames.WelcomeToTheVillage;

        public override string QuestTastText => $"Talk to everyone: {talkedNPC}/{npcCount}";

        private int talkedNPC = 0;
        private int npcCount = 4;

        public override void OnAccept()
        {
            QuestsManager.Instance.RemoveQuest(QuestsNames.GoToTheVillage, true);
        }

        public override void Logic()
        {
            //The logic that works all the time while the quest is active
        }

        public override void Progress()
        {
            talkedNPC++;

            if (talkedNPC == npcCount)
            {
                Complete();
            }
        }

        public override void OnComplete()
        {
            //Called when a quest is completed
        }
    }
}