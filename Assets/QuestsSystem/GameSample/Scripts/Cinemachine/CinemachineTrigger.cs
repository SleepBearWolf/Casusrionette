// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using Cinemachine;
using UnityEngine;

namespace QuestGameSample
{
    [RequireComponent(typeof(BoxCollider))]
    public class CinemachineTrigger : MonoBehaviour
    {
        //Selected camera from scene in editor
        [SerializeField] private CinemachineVirtualCamera m_Camera;

        private void Start()
        {
            //Register camera in switcher list 
            CinemachineCameraSwitcher.RegisterCamera(m_Camera);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out PlayerMove playerMove))
            {
                //Switch to this camera
                CinemachineCameraSwitcher.SwitchCamera(m_Camera);
            }
        }
    }
}
