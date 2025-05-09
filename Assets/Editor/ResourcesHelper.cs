using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ResourcesHelper : AssetPostprocessor
{
    public const string ROOT_PATH = "Assets/Resources";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        try
        {
            AssetDatabase.StartAssetEditing();
            foreach (string path in importedAssets.Concat(movedAssets).Concat(movedFromAssetPaths))
            {
                ResourcesAsset asset = AssetDatabase.LoadAssetAtPath<ResourcesAsset>(path);
                if (asset == null) return;

                if (path.StartsWith(ROOT_PATH))
                {
                    string filename = GetFilename(path);

                    if (filename != asset.filename)
                    {
                        asset.filename = filename;
                        EditorUtility.SetDirty(asset);
                        AssetDatabase.SaveAssetIfDirty(asset);
                    }
                }
                else Debug.LogError($"'Item {path}' is not in the items directory", asset);
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }
    }

    public static string GetFilename(string path)
    {
        string relative = Path.GetRelativePath(ROOT_PATH, path);
        return relative.Substring(0, relative.LastIndexOf('.'));
    }
}