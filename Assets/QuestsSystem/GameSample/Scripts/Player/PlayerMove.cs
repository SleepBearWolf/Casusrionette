// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using UnityEngine;

namespace QuestGameSample
{
    /// <summary>
    /// Click to move system
    /// </summary>
    public class PlayerMove : MonoBehaviour
    {
        public static PlayerMove Instance { get; private set; } //Simple singleton

        [Header("Parameters")]
        [SerializeField] private float moveSpeed = 10;
        private Vector3 movePosition;
        private Vector3 oldPosition;

        //Way status
        // (currentWayLenght == 0) = The player starts moving
        // (currentWayLenght == 1) = The player complete moving
        private float currentWayLenght = 1;
        public bool IsMove => currentWayLenght < 1;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (GameSample.Instance.IsGame)
            {
                Move();
            }
        }

        /// <summary>
        /// Sets the end point of the path
        /// Starts moving
        /// </summary>
        /// <param name="newPos">End point</param>
        public void MoveTo(Vector3 newPos)
        {
            transform.LookAt(newPos);

            oldPosition = transform.position;
            movePosition = newPos;
            currentWayLenght = 0;
        }

        /// <summary>
        /// Simple movement with Lerp
        /// </summary>
        private void Move()
        {
            if (IsMove)
            {
                transform.position = Vector3.Lerp(oldPosition, movePosition, currentWayLenght);
                currentWayLenght += Time.deltaTime * moveSpeed;
            }
        }
    }
}