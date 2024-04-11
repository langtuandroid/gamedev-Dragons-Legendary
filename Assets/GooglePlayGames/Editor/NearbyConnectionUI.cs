﻿
#if UNITY_ANDROID

namespace GooglePlayGames.Editor
{
    using UnityEngine;
    using UnityEditor;

    public class NearbyConnectionUI : EditorWindow
    {
        private string mNearbyServiceId = string.Empty;

        [MenuItem("Window/Google Play Games/Setup/Nearby Connections setup...", false, 3)]
        public static void MenuItemNearbySetup()
        {
            EditorWindow window = EditorWindow.GetWindow(
                typeof(NearbyConnectionUI), true, GPGSStrings.NearbyConnections.Title);
            window.minSize = new Vector2(400, 200);
        }

        [MenuItem("Window/Google Play Games/Setup/Nearby Connections setup...", true)]
        public static bool EnableNearbyMenuItem()
        {
#if UNITY_ANDROID
            return true;
#else
            return false;
#endif
        }

        public void OnEnable()
        {
            mNearbyServiceId = GPGSProjectSettings.Instance.Get(GPGSUtil.SERVICEIDKEY);
        }

        public void OnGUI()
        {
            GUI.skin.label.wordWrap = true;
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.Label(GPGSStrings.NearbyConnections.Blurb);
            GUILayout.Space(10);

            GUILayout.Label(GPGSStrings.Setup.NearbyServiceId, EditorStyles.boldLabel);
            GUILayout.Space(10);
            GUILayout.Label(GPGSStrings.Setup.NearbyServiceBlurb);
            mNearbyServiceId = EditorGUILayout.TextField(GPGSStrings.Setup.NearbyServiceId,
                mNearbyServiceId, GUILayout.Width(350));

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(GPGSStrings.Setup.SetupButton,
                GUILayout.Width(100)))
            {
                DoSetup();
            }

            if (GUILayout.Button("Cancel", GUILayout.Width(100)))
            {
                this.Close();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.EndVertical();
        }

        private void DoSetup()
        {
            if (PerformSetup(mNearbyServiceId, true))
            {
                EditorUtility.DisplayDialog(GPGSStrings.Success,
                    GPGSStrings.NearbyConnections.SetupComplete, GPGSStrings.Ok);
                this.Close();
            }
        }

        /// Provide static access to setup for facilitating automated builds.
        /// <param name="nearbyServiceId">The nearby connections service Id</param>
        /// <param name="androidBuild">true if building android</param>
        public static bool PerformSetup(string nearbyServiceId, bool androidBuild)
        {
            // check for valid app id
            if (!GPGSUtil.LooksLikeValidServiceId(nearbyServiceId))
            {
                if (EditorUtility.DisplayDialog(
                    "Remove Nearby connection permissions?  ",
                    "The service Id is invalid.  It must follow package naming rules.  " +
                    "Do you want to remove the AndroidManifest entries for Nearby connections?",
                    "Yes",
                    "No"))
                {
                    GPGSProjectSettings.Instance.Set(GPGSUtil.SERVICEIDKEY, null);
                    GPGSProjectSettings.Instance.Save();
                }
                else
                {
                    return false;
                }
            }
            else
            {
                GPGSProjectSettings.Instance.Set(GPGSUtil.SERVICEIDKEY, nearbyServiceId);
                GPGSProjectSettings.Instance.Save();
            }

            if (androidBuild)
            {
                // create needed directories
                GPGSUtil.EnsureDirExists("Assets/Plugins");
                GPGSUtil.EnsureDirExists("Assets/Plugins/Android");

                // Generate AndroidManifest.xml
                GPGSUtil.GenerateAndroidManifest();

                GPGSProjectSettings.Instance.Set(GPGSUtil.NEARBYSETUPDONEKEY, true);
                GPGSProjectSettings.Instance.Save();
                
                AssetDatabase.Refresh();
                
            }

            return true;
        }
    }
}
#endif