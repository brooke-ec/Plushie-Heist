using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitcher
{
    private Dictionary<int, Material[]> originalMaterials = new Dictionary<int, Material[]>();
    private Material previousMaterial = null;
    private readonly GameObject root;

    public MaterialSwitcher(GameObject root)
    {
        this.root = root;
    }

    public void Reset()
    {
        Switch(null);
    }

    public void Switch(Material material)
    {
        if (material == previousMaterial) return;

        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>())
        {
            if (previousMaterial == null) originalMaterials[renderer.GetInstanceID()] = renderer.materials;

            if (material == null && originalMaterials.TryGetValue(renderer.GetInstanceID(), out Material[] original))
                renderer.materials = original;
            else
                renderer.material = material;
        }

        previousMaterial = material;
    }
}
