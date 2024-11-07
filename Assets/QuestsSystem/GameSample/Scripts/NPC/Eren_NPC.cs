// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using QuestsSystem;

namespace QuestGameSample
{
    public class Eren_NPC : InteractiveObject
    {
        private bool talked;

        protected override void OnInteractive()
        {
            Talk();
        }

        private void Talk()
        {
            if (!talked)
            {
                QuestsManager.Instance.AddQuest(QuestsNames.GetAthleticLikeEren);
                talked = true;
            }
        }
    }
}