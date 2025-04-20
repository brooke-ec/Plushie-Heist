using System.Collections.Generic;
using UnityEngine;

public class FurnitureItem : MonoBehaviour
{
    [SerializeField] private Material invalidMaterial;
    public Vector2Int size;

    public Region region { get; private set; }
    public FurnitureGrid grid { get; private set; }

    private Dictionary<int, Material> originalMaterials = new Dictionary<int, Material>();
    private Material previousMaterial = null;

    public void Rotate()
    {
        size = new Vector2Int(size.y, size.x);
        transform.Rotate(0, 90, 0);

        Place(grid, region.center);
    }

    public void Place(FurnitureGrid grid, Vector2 coordinates)
    {
        this.grid = grid;
        region = new Region().FromSize(
            Mathf.RoundToInt(coordinates.x - size.x / 2),
            Mathf.RoundToInt(coordinates.y - size.y / 2),
            size.x, size.y
        );

        if (region.Within(grid.size))
        {
            transform.position = grid.ToWorldspace(region.center);
            SetMaterial(IsValid() ? null : invalidMaterial);
        }
    }

    public bool IsValid()
    {
        return grid != null && region != null && region.Within(grid.size) && !grid.Intersects(region);
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
