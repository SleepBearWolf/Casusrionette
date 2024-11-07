// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using QuestGameSample;

namespace QuestsSystem
{
    public class EndYourStory_QuestLogic : QuestLogic
    {
        public override QuestsNames QuestName => QuestsNames.EndYourStory;

        public override string QuestTastText => $"Complete quests: {completedQuests}/{questsCount}";

        private int completedQuests = 0;
        private int questsCount = 3;

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
            completedQuests++;

            if (completedQuests == questsCount)
            {
                Complete();
            }
        }

        public override void OnComplete()
        {
            GameSample.Instance.EndGame();
        }
    }
}