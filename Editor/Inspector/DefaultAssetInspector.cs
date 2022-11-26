using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GiraffeStarEditor
{
    [CustomEditor(typeof(DefaultAsset))]
    class DefaultAssetInspector : Editor
    {
        class BlankInspector : Editor
        {

        }

        static Type[] customAssetTypes;

        Editor editor;

        [InitializeOnLoadMethod]
        static void Init()
        {
            customAssetTypes = GetCustomAssetTypes();
        }

        void OnEnable()
        {
            var assetPath = AssetDatabase.GetAssetPath(target);
            var extension = Path.GetExtension(assetPath);
            var customAssetEditorType = GetCustomAssetEditorType(extension);

            editor = CreateEditor(target, customAssetEditorType != null ? customAssetEditorType : typeof(BlankInspector));
        }

        public override void OnInspectorGUI()
        {
            if (editor != null)
            {
                GUI.enabled = true;
                editor.OnInspectorGUI();
            }
        }

        public override bool HasPreviewGUI()
        {
            return editor != null ? editor.HasPreviewGUI() : base.HasPreviewGUI();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (editor != null)
            {
                editor.OnPreviewGUI(r, background);
            }
        }

        public override void OnPreviewSettings()
        {
            if (editor != null)
            {
                editor.OnPreviewSettings();
            }
        }

        public override string GetInfoString()
        {
            return editor != null ? editor.GetInfoString() : base.GetInfoString();
        }

        static Type[] GetCustomAssetTypes()
        {
            var currentAssembly = Assembly.GetCallingAssembly();
            var projectPath = Application.dataPath.Remove(Application.dataPath.Length - 6, 6);
            var assemblyPaths = Directory.GetFiles(projectPath + "Library/ScriptAssemblies", ".dll");

            var types = new List<Type>();
            var assetTypes = new List<Type>();

            types.AddRange(currentAssembly.GetTypes());

            foreach (var assembly in assemblyPaths.Select(Assembly.LoadFile))
            {
                types.AddRange(assembly.GetTypes());
            }

            foreach (var type in types)
            {
                var customAttributes =
                    type.GetCustomAttributes(typeof(CustomAssetAttribute), false) as CustomAssetAttribute[];

                if (customAttributes.Length > 0)
                {
                    assetTypes.Add(type);
                }
            }

            return assetTypes.ToArray();
        }

        Type GetCustomAssetEditorType(string extension)
        {
            foreach (var type in customAssetTypes)
            {
                var customAttributes =
                    type.GetCustomAttributes(typeof(CustomAssetAttribute), false) as CustomAssetAttribute[];

                foreach (var customAttribute in customAttributes)
                {
                    if (customAttribute.Extensions.Contains(extension))
                    {
                        return type;
                    }
                }
            }

            return null;
        }
    }
}