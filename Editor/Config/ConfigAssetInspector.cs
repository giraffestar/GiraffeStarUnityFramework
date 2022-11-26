using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GiraffeStarEditor
{
    [CustomAsset(".gsconfig")]
    public class ConfigAssetInspector : Editor
    {
        const float TOGGLE_WIDTH = 20f;
        const float KEY_TEXTFIELD_WIDTH = 120f;
        const float DELETE_BUTTON_WIDTH = 30f;

        ConfigEditorAsset asset;

        void OnEnable()
        {
            Load();
        }

        public override void OnInspectorGUI()
        {
            var shouldOpenDialog = false;

            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.Space(TOGGLE_WIDTH, false);
                    EditorGUILayout.LabelField("Key", GUILayout.MaxWidth(KEY_TEXTFIELD_WIDTH));
                    EditorGUILayout.LabelField("Value");
                }

                EntryPair deleteTarget = null;

                foreach (var entry in asset.Entries)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        entry.IsActive = EditorGUILayout.Toggle(entry.IsActive, GUILayout.MaxWidth(TOGGLE_WIDTH));
                        entry.Key = EditorGUILayout.DelayedTextField(entry.Key, entry.IsActive ? EditorStyles.textField : EditorStyles.helpBox, GUILayout.MaxWidth(KEY_TEXTFIELD_WIDTH));

                        var lowered = entry.Value.ToLower();
                        if (lowered == "true" || lowered == "false")
                        {
                            var valueToggle = lowered == "true";
                            valueToggle = EditorGUILayout.Toggle(valueToggle, EditorStyles.toggle, GUILayout.MaxWidth(TOGGLE_WIDTH));
                            entry.Value = valueToggle ? "True" : "False";
                            entry.Value = EditorGUILayout.DelayedTextField(entry.Value, EditorStyles.helpBox);
                        }
                        else
                        {
                            entry.Value = EditorGUILayout.DelayedTextField(entry.Value, entry.IsActive ? EditorStyles.textField : EditorStyles.helpBox);
                        }

                        if (GUILayout.Button("-", GUILayout.MaxWidth(DELETE_BUTTON_WIDTH)))
                        {
                            deleteTarget = entry;
                        }
                    }
                }

                if (deleteTarget != null)
                {
                    asset.Entries.Remove(deleteTarget);
                }

                if (GUILayout.Button("+"))
                {
                    asset.Entries.Add(new EntryPair()
                    {
                        IsActive = true,
                        Key = string.Empty,
                        Value = string.Empty,
                    });
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Save"))
                    {
                        if (Validate())
                        {
                            Save();
                        }
                        else
                        {
                            shouldOpenDialog = true;
                        }
                    }

                    if (GUILayout.Button("Revert"))
                    {
                        Load();
                    }
                }
            }

            if (shouldOpenDialog)
            {
                EditorUtility.DisplayDialog("Duplicate Config Keys",
                    "Cannot have duplicate keys. Please change key names first", "Ok");
            }
        }

        bool Validate()
        {
            var keys = new HashSet<string>();

            foreach (var entry in asset.Entries)
            {
                if (keys.Contains(entry.Key))
                {
                    return false;
                }

                keys.Add(entry.Key);
            }

            return true;
        }

        void Load()
        {
            var assetPath = AssetDatabase.GetAssetPath(target);
            var serialized = File.ReadAllText(assetPath);
            asset = JsonUtility.FromJson<ConfigEditorAsset>(serialized);
        }

        void Save()
        {
            var assetPath = AssetDatabase.GetAssetPath(target);
            var serialized = JsonUtility.ToJson(asset, true);
            File.WriteAllText(assetPath, serialized);

            var settings = new Settings();
            if (settings.Load())
            {
                var fileName = Path.GetFileNameWithoutExtension(assetPath);
                var configName = settings.GetSetting(Settings.CONFIG_KEY);
                if (fileName == configName)
                {
                    ConfigUtility.SaveTargetConfigAsset(asset);
                }
            }

            AssetDatabase.Refresh();
        }
    }
}