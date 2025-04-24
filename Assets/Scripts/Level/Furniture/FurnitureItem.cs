using System.Collections.Generic;
using UnityEngine;

public class FurnitureItem : MonoBehaviour
{
    [field: SerializeReference] public Vector2Int size { get; private set; }

    public Vector2Int position { get; private set; }
    public int rotation { get; private set; } = 0;
    public FurnitureGrid owner { get; set; }

    public Region region => new Region().FromSize(position, size);

    private Dictionary<int, Material> originalMaterials = new Dictionary<int, Material>();
    private Material previousMaterial = null;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null) return;
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(size.x, 0, size.y) * FurnitureSettings.instance.cellSize);
    }
#endif

    public void Rotate()
    {
        size = new Vector2Int(size.y, size.x);
        transform.Rotate(0, 90, 0);
    }

    public void Move(Vector2Int target)
    {
        position = Util.Clamp(target, Vector2Int.zero, owner.size - size);

        transform.position = owner.ToWorldspace(region.center);
        SetMaterial(IsValid() ? null : FurnitureSettings.instance.invalidMaterial);
    }

    public bool IsValid()
    {
        return owner != null && position != null && region.Within(owner.size) && !owner.Intersects(region);
    }

    public void SetMaterial(Material material)
    {
        if (material == previousMaterial) return;

        foreach (Renderer renderer in transform.GetComponentsInChildren<Renderer>())
        {
            if (previousMaterial == null) originalMaterials[renderer.GetInstanceID()] = renderer.material;

            if (material == null && originalMaterials.TryGetValue(renderer.GetInstanceID(), out Material original))
                renderer.material = original;
            else
                renderer.material = material;
        }
        
        previousMaterial = material;
    }
}
