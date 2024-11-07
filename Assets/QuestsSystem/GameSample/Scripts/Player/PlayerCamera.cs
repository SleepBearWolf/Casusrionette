// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using UnityEngine;

namespace QuestGameSample
{
    /// <summary>
    /// Click to move system
    /// </summary>
    public class PlayerCamera : MonoBehaviour
    {
        private Camera playerCamera;
        private LayerMask layerMask;

        private void Awake()
        {
            playerCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            //Works only in Game state
            if (Input.GetMouseButtonDown(0) && GameSample.Instance.IsGame)
            {
                MoveToMousePos();
            }
        }

        /// <summary>
        /// Using raycast to set position
        /// Call player move and send position to move
        /// </summary>
        private void MoveToMousePos()
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 500))
            {
                if (hit.collider.gameObject.name == "Ground")
                {
                    PlayerMove.Instance.MoveTo(hit.point);
                }
            }
        }
    }
}