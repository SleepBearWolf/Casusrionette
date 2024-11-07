// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuestsSystem.CustomEditor
{
    /// <summary>
    /// Custom Progress bar visual element
    /// </summary>
    public class CustomProgressBar : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<CustomProgressBar, UxmlTraits> { }

        private VisualElement progressIndicator;

        /// <summary>
        /// When called in case of not initialized components, 
        /// it initialize them
        /// </summary>
        private VisualElement ProgressIndicator
        {
            get
            {
                if (progressIndicator == null)
                {
                    progressIndicator = contentContainer.Q<VisualElement>("Progress-Indicator");
                }

                return progressIndicator;
            }
        }

        private Label progressStateText;

        /// <summary>
        /// When called in case of not initialized components, 
        /// it initialize them
        /// </summary>
        private Label ProgressStateText
        {
            get
            {
                if (progressStateText == null)
                {
                    progressStateText = contentContainer.Q<Label>("ProgressState-Text");
                }

                return progressStateText;
            }
        }

        private bool enabled;

        /// <summary>
        /// Using a constructor instead of Start/Awake
        /// </summary>
        public CustomProgressBar()
        {
            string progressBarUxmlPath = EditorUtilities.FindAssetPath<VisualTreeAsset>("Custom-Progress-Bar", ".uxml"); //Find uxml file in project by using utilities
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(progressBarUxmlPath);

            VisualElement rootFromUXML = visualTree.CloneTree();
            contentContainer.Add(rootFromUXML);
        }

        /// <summary>
        /// Update progress bar indicator
        /// </summary>
        /// <param name="stateInfo">Progress info for ui drawing</param>
        /// <param name="progress">Porgress percent: min: 0, max: 100</param>
        public void UpdateProgress(string stateInfo, int progress)
        {
            //If indicator is not enabled then enable
            if (!enabled)
            {
                contentContainer.style.display = DisplayStyle.Flex;
                enabled = true;
            }

            int clampedProgress = Mathf.Clamp(progress, 0, 100); //Clamp progress 
            ProgressIndicator.style.width = Length.Percent(clampedProgress);
            ProgressStateText.text = stateInfo;
        }

        /// <summary>
        /// Disabling progress bar
        /// </summary>
        public void Disable()
        {
            contentContainer.style.display = DisplayStyle.None;
            enabled = false;
        }
    }
}
