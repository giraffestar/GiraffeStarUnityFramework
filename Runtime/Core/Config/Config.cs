using UnityEngine;

namespace GiraffeStar
{
    public partial class Config
    {
        public const string CONFIG_NAME = "ConfigName";
        public const string CONFIG_PATH = "GiraffeStar/Config";

        ConfigAsset asset;

        public Config()
        {
            asset = new ConfigAsset();
        }

        public bool GetBoolOrDefaultInternal(string key, bool defaultValue = false)
        {
            var entry = asset.GetEntry(key);
            return entry?.GetBoolOrDefault(defaultValue) ?? defaultValue;
        }

        public int GetIntOrDefaultInternal(string key, int defaultValue = 0)
        {
            var entry = asset.GetEntry(key);
            return entry?.GetIntOrDefault(defaultValue) ?? defaultValue;
        }

        public float GetFloatOrDefaultInternal(string key, float defaultValue = 0f)
        {
            var entry = asset.GetEntry(key);
            return entry?.GetFloatOrDefault(defaultValue) ?? defaultValue;
        }

        public string GetStringOrDefaultInternal(string key, string defaultValue = null)
        {
            var entry = asset.GetEntry(key);
            return entry?.GetString() ?? defaultValue;
        }

        public void Load()
        {
            var serialized = Resources.Load<TextAsset>(CONFIG_PATH);

            if (serialized != null)
            {
                asset = new ConfigAsset(serialized.text);
            }
        }
    }
}