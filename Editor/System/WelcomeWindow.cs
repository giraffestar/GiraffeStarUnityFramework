using UnityEditor;
using UnityEngine;

namespace GiraffeStarEditor
{
    /// <summary>
    /// Welcome page window script, introducing setup and guides
    /// </summary>
    class WelcomeWindow : EditorWindow
    {
        [UnityEditor.Callbacks.DidReloadScripts]
        static void OnScriptsReloaded()
        {
            var settings = new Settings();
            if (!settings.Load())
            {
                Init();
            }
        }

        static void Init()
        {
            var window = (WelcomeWindow) EditorWindow.GetWindowWithRect(
                typeof(WelcomeWindow), new Rect(Vector2.zero, new Vector2(350f, 80f)), false, "Get Started");
            
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Thanks for downloading GiraffeStar Unity Framework!");
            EditorGUILayout.LabelField("Press the Setup button to get started.");

            if (GUILayout.Button("Setup"))
            {
                SetupMenu.Setup();
            }
        }
    }
}