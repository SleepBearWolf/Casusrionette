// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using QuestsSystem;

namespace QuestGameSample
{
    public class Joe_NPC : InteractiveObject
    {
        protected override void OnInteractive()
        {
            Talk();
        }

        private void Talk()
        {
            QuestsManager.Instance.RemoveQuest(QuestsNames.TalkToJoe, true);
            QuestsManager.Instance.UpdateQuestProgress(QuestsNames.EndYourStory);
        }
    }
}