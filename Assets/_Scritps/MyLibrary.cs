using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Cinemachine;

namespace MyLibrary
{
    public class AuthUtils
    {
        
    }

    // public class GameUtils
    // {
    //     public static void CamFollower(Transform playerTran)
    //     {
    //         CinemachineVirtualCamera virtualCamera = Object.FindObjectOfType<CinemachineVirtualCamera>();

    //         if (virtualCamera != null && playerTran != null)
    //             virtualCamera.Follow = playerTran;
    //         else
    //             Debug.Log("CinemachineVirtualCamera or Player not found.");
    //     }
    // }

    public class ControlUtils
    {
        public static void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}