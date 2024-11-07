// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using Cinemachine;
using System.Collections.Generic;

namespace QuestGameSample
{
    public class CinemachineCameraSwitcher
    {
        //List of registered cameras
        private static List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();

        /// <summary>
        /// Adds a camera to the general list
        /// </summary>
        /// <param name="camera">Registrable camera</param>
        public static void RegisterCamera(CinemachineVirtualCamera camera)
        {
            if (!cameras.Contains(camera)) //Non duplicate check
            {
                cameras.Add(camera);
            }
        }

        /// <summary>
        /// Switching between cameras from the list
        /// </summary>
        /// <param name="cameraTo">New priority camera</param>
        public static void SwitchCamera(CinemachineVirtualCamera cameraTo)
        {
            foreach (CinemachineVirtualCamera camera in cameras)
            {
                //If camera from list is target camera set 1 in priority
                camera.Priority = (camera == cameraTo) ? 1 : 0;
            }
        }
    }
}