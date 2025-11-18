using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Editor
{
    public static class ScenesCodegen
    {
        private const string OutputPath = "Assets/Scripts/Shared/Generated/Scenes.cs";

        [MenuItem("Tools/Generate Addressable Scenes Consts")]
        public static void Generate()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("[ScenesCodegen] Addressables settings not found!");
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("// Auto-generated from Addressables, do not edit by hand");
            sb.AppendLine("public static class Scenes");
            sb.AppendLine("{");

            foreach (var group in settings.groups)
            {
                if (group == null) continue;

                foreach (var entry in group.entries)
                {
                    if (entry == null) continue;

                    // Проверяем, что это именно сцена
                    if (entry.AssetPath.EndsWith(".unity"))
                    {
                        string sceneName = Path.GetFileNameWithoutExtension(entry.AssetPath);
                        sb.AppendLine($"    public const string {sceneName} = \"{sceneName}\";");
                    }
                }
            }

            sb.AppendLine("}");

            string newContent = sb.ToString();
            string oldContent = File.Exists(OutputPath) ? File.ReadAllText(OutputPath) : "";

            if (newContent != oldContent)
            {
                File.WriteAllText(OutputPath, newContent);
                AssetDatabase.Refresh();
                Debug.Log($"[ScenesCodegen] Updated {OutputPath}");
            }
            else
            {
                Debug.Log("[ScenesCodegen] No changes, skipped");
            }
        }
    }
}