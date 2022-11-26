using System.Collections.Generic;

namespace GiraffeStar
{
    public class ConfigAsset
    {
        readonly Dictionary<string, ConfigEntry> entriesByKey;

        public ConfigAsset()
        {
            entriesByKey = new Dictionary<string, ConfigEntry>();
        }

        public ConfigAsset(string serialized)
        {
            entriesByKey = new Dictionary<string, ConfigEntry>();

            var deserialized = JsonUtilityEx.DictionaryFromJson<string, string>(serialized);
            foreach (var pair in deserialized)
            {
                entriesByKey.Add(pair.Key, new ConfigEntry(pair.Value));
            }
        }

        public void SetOrAdd(string key, string value)
        {
            if (entriesByKey.ContainsKey(key))
            {
                entriesByKey[key] = new ConfigEntry(value);
            }
            else
            {
                entriesByKey.Add(key, new ConfigEntry(value));
            }
        }

        public ConfigEntry GetEntry(string key)
        {
            if (entriesByKey.ContainsKey(key))
            {
                return entriesByKey[key];
            }

            return null;
        }

        public string Serialize()
        {
            var serialized = new Dictionary<string, string>();
            foreach (var entry in entriesByKey)
            {
                serialized.Add(entry.Key, entry.Value.GetString());
            }

            return JsonUtilityEx.DictionaryToJson(serialized, true);
        }
    }
}