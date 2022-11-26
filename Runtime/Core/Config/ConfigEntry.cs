namespace GiraffeStar
{
    public class ConfigEntry
    {
        enum EntryType
        {
            String,
            Boolean,
            Integer,
            Float,
        }

        readonly string raw;
        
        EntryType entryType;
        bool boolValue;
        int intValue;
        float floatValue;

        public ConfigEntry(string rawValue)
        {
            raw = rawValue;
            Parse(rawValue);
        }

        public bool GetBoolOrDefault(bool defaultValue = false)
        {
            return entryType == EntryType.Boolean ? boolValue : defaultValue;
        }

        public int GetIntOrDefault(int defaultValue = 0)
        {
            return entryType == EntryType.Integer ? intValue : defaultValue;
        }

        public float GetFloatOrDefault(float defaultValue = 0f)
        {
            return entryType == EntryType.Float ? floatValue : defaultValue;
        }

        public string GetString()
        {
            return raw;
        }

        void Parse(string rawValue)
        {
            if (rawValue.ToLower() == "true")
            {
                boolValue = true;
                entryType = EntryType.Boolean;
            }
            else if (rawValue.ToLower() == "false")
            {
                boolValue = false;
                entryType = EntryType.Boolean;
            }
            else if (int.TryParse(rawValue, out var intResult))
            {
                intValue = intResult;
                entryType = EntryType.Integer;
            }
            else if (float.TryParse(rawValue, out var floatResult))
            {
                floatValue = floatResult;
                entryType = EntryType.Float;
            }
            else
            {
                entryType = EntryType.String;
            }
        }
    }
}