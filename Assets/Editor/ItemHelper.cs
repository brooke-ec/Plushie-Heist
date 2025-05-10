using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Asset postprocessor for helping maintain references between <see cref="FurnitureItem"/> and <see cref="FurnitureController"/>.
/// </summary>
public class ItemHelper : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        try
        {
            AssetDatabase.StartAssetEditing();
            List<FurnitureController> sources = new List<FurnitureController>();
            importedAssets.Concat(movedAssets).Concat(movedFromAssetPaths).ForEach(path =>
            {
                // If furniture prefab, defer processing until later
                if (TryLoad(out GameObject prefab, path) && prefab.TryGetComponent(out FurnitureController source)) sources.Add(source);
                else if (TryLoad(out FurnitureItem item, path)) ProcessFurnitureItem(item);
            });

            sources.ForEach(ProcessFurnitureSource);
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }
    }

    private static void ProcessFurnitureSource(FurnitureController source)
    {
        // Create or link item asset
        if (source.item == null)
        {
            string path = FurnitureItem.FULL_PATH + source.name + ".asset";
            FurnitureItem asset = AssetDatabase.LoadAssetAtPath<FurnitureItem>(path);

            if (asset == null && !AssetDatabase.IsAssetImportWorkerProcess())
            {
                Debug.Log($"Creating item asset for {source.name}");
                asset = ScriptableObject.CreateInstance(typeof(FurnitureItem)) as FurnitureItem;
                asset.itemName = source.name;
                asset.name = source.name;
                AssetDatabase.CreateAsset(asset, path);
            }

            source.item = asset;
            EditorUtility.SetDirty(source.gameObject);
            AssetDatabase.SaveAssetIfDirty(source.gameObject);
        }

        // Fix any one-way references
        if (source.item.prefab == null)
        {
            source.item.prefab = source;
            EditorUtility.SetDirty(source.item);
            AssetDatabase.SaveAssetIfDirty(source.item);
        }
        else if (source.item.prefab != source) Debug.LogError($"'Item controller {source.name}' references '{source.item.name}', but it references '{source.item.prefab.name}'", source);
    }

    private static void ProcessFurnitureItem(FurnitureItem item)
    {
        if (item.prefab != null)
        {
            // Fix any one-way references
            if (item.prefab.item == null)
            {
                item.prefab.item = item;
                EditorUtility.SetDirty(item.prefab);
                AssetDatabase.SaveAssetIfDirty(item.prefab);
            }
            else if (item.prefab.item != item) Debug.LogError($"Item '{item.name}' references '{item.prefab.name}', but it references '{item.prefab.item.name}'", item);
        }
        else Debug.LogError($"Item '{item.name}' has no referenced prefab", item);
    }

    private static bool TryLoad<T>(out T asset, string path) where T : Object
    {
        asset = AssetDatabase.LoadAssetAtPath<T>(path);
        return asset != null;
    }

}
