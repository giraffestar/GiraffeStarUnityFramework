using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GiraffeStarEditor
{
    class ConfigManagerWindow : EditorWindow
    {
        List<string> names;
        string[] converted;
        int selectedIndex;
        bool loadSuccess = false;

        public ConfigManagerWindow()
        {
            names = new List<string>();
        }

        [MenuItem("GiraffeStar/Config/ConfigManager")]
        static void Init()
        {
            var window = (ConfigManagerWindow) EditorWindow.GetWindow(typeof(ConfigManagerWindow), false, "Config Manager");
            window.Show();
        }

        void OnEnable()
        {
            if (!new Settings().Load())
            {
                return;
            }

            loadSuccess = true;

            Load();
        }

        void OnGUI()
        {
            if (!loadSuccess)
            {
                EditorGUILayout.LabelField("Cannot find EditorSettings file. Run GiraffeStar/Setup first.");
                return;
            }

            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.LabelField("Editor is compiling...");
                return;
            }

            using (new EditorGUILayout.VerticalScope())
            {
                selectedIndex = GUILayout.SelectionGrid(selectedIndex, converted, 1, EditorStyles.radioButton);
                
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Save"))
                    {
                        var targetConfig = converted[selectedIndex];
                        ConfigUtility.SetAsTargetConfigAsset(targetConfig);
                    }

                    if (GUILayout.Button("Revert"))
                    {
                        Load();
                    }
                }
            }
        }

        void Load()
        {
            names.Clear();

            var configFiles = Directory.GetFiles(PathConstants.CONFIG_FOLDER_PATH, "*" + PathConstants.CONFIG_EXTENSION);
            var configNames = new List<string>();

            foreach (var configFile in configFiles)
            {
                var split = configFile.Split(Path.DirectorySeparatorChar);
                var configName = split[split.Length - 1].Split('.')[0];
                configNames.Add(configName);
            }

            var defaultNames = new List<string>()
            {
                "Dev",
                "Test",
                "Release",
            };

            // default name 우선
            foreach (var defaultName in defaultNames)
            {
                if (configNames.Contains(defaultName))
                {
                    names.Add(defaultName);
                }
            }

            foreach (var configName in configNames)
            {
                if (defaultNames.Contains(configName))
                {
                    continue;
                }

                names.Add(configName);
            }

            converted = names.ToArray();

            var settings = new Settings();
            if (settings.Load())
            {
                var targetConfig = settings.GetSetting(Settings.CONFIG_KEY);

                selectedIndex = names.FindIndex(x => x == targetConfig);
                if (selectedIndex == -1)
                {
                    selectedIndex = 0;
                }
            }
        }
    }
}