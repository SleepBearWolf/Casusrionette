// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using UnityEngine.UIElements;

namespace QuestGameSample
{
    /// <summary>
    /// I decided to use UI without the help of MonoBehaviour
    /// </summary>
    public class GameSampleUI : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<GameSampleUI, UxmlTraits> { }

        private static GameSampleUI instance;
        /// <summary>
        /// When called in case of not initialized components, 
        /// it initialize them
        /// </summary>
        public static GameSampleUI Instance
        {
            get
            {
                if (!instance.initialised) //Isitialize check
                {
                    instance.InitComponents();
                }

                return instance;
            }

            private set { instance = value; }
        }

        public bool initialised;

        //List of all screens
        private VisualElement startScreen, gameScreen, endScreen;

        /// <summary>
        /// Using a constructor instead of Start/Awake
        /// </summary>
        public GameSampleUI()
        {
            Instance = this;
        }

        /// <summary>
        /// Launches the start screen
        /// </summary>
        public void StartScreen()
        {
            startScreen.style.display = DisplayStyle.Flex;
        }

        /// <summary>
        /// Initialization of all components
        /// </summary>
        private void InitComponents()
        {
            startScreen = contentContainer.Q<VisualElement>("Start-Screen");
            gameScreen = contentContainer.Q<VisualElement>("Game-Screen");
            endScreen = contentContainer.Q<VisualElement>("End-Screen");

            startScreen.Q<Button>("Play-Button").clicked += StartGame;

            initialised = true;
        }

        /// <summary>
        /// Disabling all screens for the further display of one
        /// </summary>
        private void DisableAllScreens()
        {
            startScreen.style.display = DisplayStyle.None;
            gameScreen.style.display = DisplayStyle.None;
            endScreen.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Activates the game screen
        /// </summary>
        public void StartGame()
        {
            DisableAllScreens();
            gameScreen.style.display = DisplayStyle.Flex;

            GameSample.Instance.StartGame();
        }

        /// <summary>
        /// Activates the end game screen
        /// </summary>
        public void EndGame()
        {
            DisableAllScreens();
            endScreen.style.display = DisplayStyle.Flex;
        }
    }
}