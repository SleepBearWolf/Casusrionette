// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using QuestsSystem;
using UnityEngine;

namespace QuestGameSample
{
    public class GameSample : MonoBehaviour
    {
        public static GameSample Instance { get; private set; } //Simple singleton

        public bool IsGame { get; private set; } //Global game status variable

        private void Awake()
        {
            Instance = this; //Init instance
        }

        private void Start()
        {
            //Enable start screen in game ui
            GameSampleUI.Instance.StartScreen();
        }

        /// <summary>
        /// Launching the game
        /// </summary>
        public void StartGame()
        {
            IsGame = true;

            //Activate quests
            QuestsManager.Instance.AddQuest(QuestsNames.EndYourStory);
            QuestsManager.Instance.AddQuest(QuestsNames.GoToTheVillage);
        }

        /// <summary>
        /// Stopping the game
        /// </summary>
        public void EndGame()
        {
            IsGame = false;

            //Enable end game screen in game ui
            GameSampleUI.Instance.EndGame();
        }
    }
}