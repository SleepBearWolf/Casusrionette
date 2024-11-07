// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using UnityEngine;

namespace QuestGameSample
{
    [RequireComponent(typeof(BoxCollider))]
    //Abstract class for custom interactive objects
    public abstract class InteractiveObject : MonoBehaviour
    {
        private bool isPlayerClose;

        /// <summary>
        /// Check for player entered in interactive zone
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerMove playerMove))
            {
                isPlayerClose = true;
            }
        }

        /// <summary>
        /// Check for player exit from interactive zone
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out PlayerMove playerMove))
            {
                isPlayerClose = false;
            }
        }

        private void Update()
        {
            //Can interactive only when player insise zone
            if (isPlayerClose)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    OnInteractive();
                }
            }
        }

        /// <summary>
        /// Custom interactive logic
        /// </summary>
        protected abstract void OnInteractive();
    }
}