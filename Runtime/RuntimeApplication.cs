using System;
using UnityEngine;

namespace com.fscigliano.CommonExtensions
{
    /// <summary>
    /// Creation Date:   27/02/2020 22:12:07
    /// Product Name:    Common extensions
    /// Developers:      Franco Scigliano
    /// Description:     Provides several missing but vital features to the use of the application in the Unity Editor
    ///                  related to detecting if the application is running in editor mode or in a build.
    /// </summary>
    public static class RuntimeApplication
    {
        public static bool isQuittingEditor = false;
        public static bool ApplicationisPlaying = false;
        public static Action OnEnterPlayMode;
        public static Action OnExitPlayMode;

        public static bool isPlaying
        {
            get
            {
                if (!Application.isEditor)
                {
                    return true;
                }
                else
                {
                    return ApplicationisPlaying;
                }
            }
        }

        public static bool isPlayingButQuitting
        {
            get { return isQuittingEditor && ApplicationisPlaying; }
        }
        public static void QuitGame()
        {
            if (Application.isEditor)
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
            }

    #if UNITY_ANDROID
		    AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
		    activity.Call<bool>("moveTaskToBack" , true); 
    #elif UNITY_IOS
		    Application.Quit();
    #endif
        }
    }

}
