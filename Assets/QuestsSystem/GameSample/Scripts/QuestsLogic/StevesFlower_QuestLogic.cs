// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

namespace QuestsSystem
{
    public class StevesFlower_QuestLogic : QuestLogic
    {
        public override QuestsNames QuestName => QuestsNames.StevesFlower;

        public override string QuestTastText => $"Collect {needFlowers} flowers, you have {collectedFlowers}";

        private int collectedFlowers = 0;
        private int needFlowers = 10;

        public override void OnAccept()
        {
            //Called when a quest is accepted
        }

        public override void Logic()
        {
            //The logic that works all the time while the quest is active
        }

        public override void Progress()
        {
            collectedFlowers++;

            if (collectedFlowers == needFlowers)
            {
                Complete();
            }
        }

        public override void OnComplete()
        {
            QuestsManager.Instance.UpdateQuestProgress(QuestsNames.EndYourStory);
        }
    }
}