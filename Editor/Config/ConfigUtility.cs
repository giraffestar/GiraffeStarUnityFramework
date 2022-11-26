using GiraffeStar;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GiraffeStarEditor
{
    static class ConfigUtility
    {
        public static void SetAsTargetConfigAsset(string configName)
        {
            var setting = new Settings();
            setting.Load();
            setting.SetSetting(Settings.CONFIG_KEY, configName);
            setting.Save();

            var configPath = $"{PathConstants.CONFIG_FOLDER_PATH}/{configName}{PathConstants.CONFIG_EXTENSION}";

            if (!File.Exists(configPath))
            {
                throw new FrameworkException($"Cannot find target({configName}) config file.");
            }

            var textAsset = File.ReadAllText(configPath);
            var configEditorAsset = JsonUtility.FromJson<ConfigEditorAsset>(textAsset);
            SaveTargetConfigAsset(configEditorAsset);
        }

        public static void SaveTargetConfigAsset(ConfigEditorAsset asset)
        {
            var configAsset = asset.Convert();
            var serialized = configAsset.Serialize();
            
            Directory.CreateDirectory(PathConstants.TARGET_CONFIG_FOLDER_PATH);
            File.WriteAllText(PathConstants.TARGET_CONFIG_FULL_PATH, serialized);
            AssetDatabase.Refresh();
        }

        public static bool TrySaveDefaultConfigEditorAsset(string fileName)
        {
            var fullPath = $"{PathConstants.CONFIG_FOLDER_PATH}/{fileName}{PathConstants.CONFIG_EXTENSION}";
            if (File.Exists(fullPath))
            {
                return false;
            }

            var asset = new ConfigEditorAsset();
            asset.Entries.Add(new EntryPair()
            {
                IsActive = true,
                Key = Config.CONFIG_NAME,
                Value = fileName,
            });
            SaveConfigEditorAsset(fileName, asset);

            return true;
        }

        public static void SaveConfigEditorAsset(string fileName, ConfigEditorAsset asset)
        {
            Directory.CreateDirectory(PathConstants.CONFIG_FOLDER_PATH);
            var serialized = JsonUtility.ToJson(asset, true);
            var fullPath = $"{PathConstants.CONFIG_FOLDER_PATH}/{fileName}{PathConstants.CONFIG_EXTENSION}";
            File.WriteAllText(fullPath, serialized);
            AssetDatabase.Refresh();
        }
    }
}