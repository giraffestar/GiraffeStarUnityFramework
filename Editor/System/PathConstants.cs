using UnityEngine;

namespace GiraffeStarEditor
{
    class PathConstants
    {
        public static readonly string CONFIG_FOLDER_PATH = $"{Application.dataPath}/GiraffeStar/Config";
        public static readonly string TARGET_CONFIG_FOLDER_PATH = $"{Application.dataPath}/Resources/GiraffeStar";
        public static readonly string TARGET_CONFIG_FULL_PATH = $"{Application.dataPath}/Resources/GiraffeStar/Config.json";
        public static readonly string SCREENSHOT_DEFAULT_FOLDER_PATH = $"{Application.dataPath}/Screenshot";

        public const string CONFIG_EXTENSION = ".gsconfig";
        public const string JSON_EXTENSION = ".json";
    }
}