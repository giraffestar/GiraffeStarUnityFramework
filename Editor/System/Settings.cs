using System.Collections.Generic;
using System.IO;
using GiraffeStar;
using UnityEditor;
using UnityEngine;

namespace GiraffeStarEditor
{
    /// <summary>
    /// Editor only settings file
    /// </summary>
    class Settings
    {
        const string FILE_NAME = "EditorSettings.gsprefs";
        public const string CONFIG_KEY = "Config";

        readonly string fullPath;
        Dictionary<string, string> settings;

        public Settings()
        {
            settings = new Dictionary<string, string>();
            fullPath = $"{Application.dataPath}/GiraffeStar/{FILE_NAME}";
        }

        public bool Load()
        {
            var hasFile = File.Exists(fullPath);

            if (hasFile)
            {
                var textAsset = File.ReadAllText(fullPath);
                settings = JsonUtilityEx.DictionaryFromJson<string, string>(textAsset);

                return true;
            }

            return false;
        }

        public void Save()
        {
            var serialized = JsonUtilityEx.DictionaryToJson(settings, true);
            Directory.CreateDirectory($"{Application.dataPath}/GiraffeStar");
            File.WriteAllText(fullPath, serialized);
            AssetDatabase.Refresh();
        }

        public void SetSetting(string key, string value)
        {
            if (settings.ContainsKey(key))
            {
                settings[key] = value;
            }
            else
            {
                settings.Add(key, value);
            }
        }

        public string GetSetting(string key)
        {
            return settings[key];
        }
    }
}