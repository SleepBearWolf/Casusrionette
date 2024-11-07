// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using QuestGameSample;
using UnityEngine;

namespace QuestsSystem
{
    public class GetAthleticLikeEren_QuestLogic : QuestLogic
    {
        public override QuestsNames QuestName => QuestsNames.GetAthleticLikeEren;

        public override string QuestTastText => $"Run through the village: {currentMeters}m/{needMeters}m";

        private float currentMeters = 0;
        private float needMeters = 350;

        private Vector3 playerLastPos;

        public override void OnAccept()
        {
            //Set last position on quest accept
            playerLastPos = PlayerMove.Instance.transform.position;
        }

        public override void Logic()
        {
            //If player is moving add meters in progress
            if (PlayerMove.Instance.IsMove)
            {
                QuestsManager.Instance.UpdateQuestProgress(QuestsNames.GetAthleticLikeEren);
            }
            else
                playerLastPos = PlayerMove.Instance.transform.position;
        }

        public override void Progress()
        {
            Vector3 playerPos = PlayerMove.Instance.transform.position;
            currentMeters += Vector3.Distance(playerLastPos, playerPos) / 100;

            if (currentMeters >= needMeters)
            {
                Complete();
            }
        }

        public override void OnComplete()
        {
            QuestsManager.Instance.UpdateQuestProgress(QuestsNames.EndYourStory);
            QuestsManager.Instance.AddQuest(QuestsNames.FindJoe);
        }
    }
}