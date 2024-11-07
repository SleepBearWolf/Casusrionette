// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using UnityEngine;

namespace QuestsSystem
{
    /// <summary>
    /// A small example of creating custom components for the quest system
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class QuestTrigger : MonoBehaviour
    {
        [SerializeField] private QuestsNames questName; //Set the required quest from inspector
        private enum TriggerType { Accept, Update, Complete }
        [SerializeField] private TriggerType triggerType;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                //Perform the desired action depending on the selected type
                switch (triggerType)
                {
                    case TriggerType.Accept:
                        QuestsManager.Instance.AddQuest(questName);
                        break;
                    case TriggerType.Update:
                        QuestsManager.Instance.UpdateQuestProgress(questName);
                        break;
                    case TriggerType.Complete:
                        QuestsManager.Instance.RemoveQuest(questName, true);
                        break;
                }

                gameObject.SetActive(false); //self-disabling
            }
        }

        /// <summary>
        /// Displaying a Trigger Zone in the Scene Window
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(0, 255, 0, 0.5f);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }
    }
}