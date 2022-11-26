namespace GiraffeStar
{
    public partial class Config
    {
        static Config config;

        public static void Init()
        {
            config = new Config();
            config.Load();
        }

        public static bool GetBoolOrDefault(string key, bool defaultValue = false)
        {
            return config.GetBoolOrDefaultInternal(key, defaultValue);
        }

        public static int GetIntOrDefault(string key, int defaultValue = 0)
        {
            return config.GetIntOrDefaultInternal(key, defaultValue);
        }

        public static float GetFloatOrDefault(string key, float defaultValue = 0f)
        {
            return config.GetFloatOrDefaultInternal(key, defaultValue);
        }

        public static string GetStringOrDefault(string key, string defaultValue = null)
        {
            return config.GetStringOrDefaultInternal(key, defaultValue);
        }
    }
}