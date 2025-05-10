using UnityEditor;
using UnityEngine;

/// <summary>
/// Helper wizard to bulk remap a material to models.
/// </summary>
public class RemapMaterials : ScriptableWizard
{
    [Space(10)]
    [SerializeField] private Material replacementMaterial;
    [SerializeField] private string materialName;

    void OnWizardUpdate()
    {
        helpString = "Select Model Files";
        isValid = replacementMaterial != null && materialName != "" && Selection.objects.Length > 0;
    }

    void OnWizardCreate()
    {
        Object[] gos = Selection.objects;
        foreach (Object go in gos)
        {
            AssetImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(go));

            if (importer is ModelImporter model)
            {
                importer.AddRemap(new AssetImporter.SourceAssetIdentifier(replacementMaterial.GetType(), materialName), replacementMaterial);
                importer.SaveAndReimport();
            }
            else Debug.LogError($"'{go.name}' is not a model.");
        }

    }

    [MenuItem("Assets/Remap Material")]
    static void CreateWindow()
    {
        DisplayWizard("Remap Materials", typeof(RemapMaterials), "Remap Materials");
    }
}