// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using QuestsSystem;

namespace QuestGameSample
{
    public class Flower_Interactive : InteractiveObject
    {
        protected override void OnInteractive()
        {
            //Check for quest existing in active quests
            if (QuestsManager.Instance.IsQuestActive(QuestsNames.StevesFlower))
            {
                gameObject.SetActive(false);
                QuestsManager.Instance.UpdateQuestProgress(QuestsNames.StevesFlower);
            }
        }
    }
}