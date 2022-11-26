using UnityEditor;
using UnityEngine;

namespace GiraffeStarEditor
{
    public class SetupMenu
    {
        [MenuItem("GiraffeStar/Setup")]
        public static void Setup()
        {
            SetupBasicConfigFiles();
            SetupSettingsFile();
        }

        static void SetupSettingsFile()
        {
            var settings = new Settings();
            if (settings.Load())
            {
                Debug.Log("Settings file already exists!");
            }
            else
            {
                ConfigUtility.SetAsTargetConfigAsset("Dev");
                Debug.Log("Setup complete.");
            }
        }

        static void SetupBasicConfigFiles()
        {
            var basics = new string[] {"Dev", "Test", "Release"};
            foreach (var basic in basics)
            {
                ConfigUtility.TrySaveDefaultConfigEditorAsset(basic);
            }
        }
    }
}