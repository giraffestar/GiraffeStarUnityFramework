using GiraffeStar;
using System;
using System.Collections.Generic;

namespace GiraffeStarEditor
{
    [Serializable]
    class EntryPair
    {
        public bool IsActive;
        public string Key;
        public string Value;
    }
    
    [Serializable]
    class ConfigEditorAsset
    {
        public List<EntryPair> Entries = new List<EntryPair>();

        public ConfigAsset Convert()
        {
            var asset = new ConfigAsset();

            foreach (var entry in Entries)
            {
                if (entry.IsActive)
                {
                    asset.SetOrAdd(entry.Key, entry.Value);
                }
            }

            return asset;
        }
    }
}