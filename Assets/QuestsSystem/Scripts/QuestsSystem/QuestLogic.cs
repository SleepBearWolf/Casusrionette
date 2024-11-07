// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

namespace QuestsSystem
{
    /// <summary>
    /// Abstract class for custom quest logic
    /// </summary>
    public abstract class QuestLogic
    {
        public abstract QuestsNames QuestName { get; }
        public abstract string QuestTastText { get; }

        /// <summary>
        /// Called when a quest is accepted
        /// </summary>
        public abstract void OnAccept();

        /// <summary>
        /// The logic that works all the time while the quest is active
        /// </summary>
        public abstract void Logic();

        /// <summary>
        /// Called when there is logic for progress (Call from any other entity)
        /// </summary>
        public abstract void Progress();

        /// <summary>
        /// Complete a quest (Only called on a child class)
        /// </summary>
        protected void Complete()
        {
            QuestsManager.Instance.RemoveQuest(QuestName, true);
        }

        /// <summary>
        /// Called when a quest is completed
        /// </summary>
        public abstract void OnComplete();
    }
}