using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class FurnitureGrid : MonoBehaviour
{
    public Vector2Int size => new Vector2Int((int)(collider.size.x / cellSize), (int)(collider.size.z / cellSize));
    private static float cellSize => FurnitureSettings.instance.cellSize;
    private static float spacing => FurnitureSettings.instance.spacing;

    private List<FurnitureItem> items = new List<FurnitureItem>();
    private GridMesh mesh = new GridMesh(Color.green);
    new private BoxCollider collider;
    private MeshFilter filter;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawMesh(mesh.Build(size, cellSize, spacing), transform.TransformPoint(collider.center));
    }

    private void OnValidate()
    {
        if (collider == null) collider = GetComponent<BoxCollider>();
    }
#endif

    private void Start()
    {
        collider = GetComponent<BoxCollider>();
        filter = GetComponent<MeshFilter>();

        gameObject.layer = LayerMask.NameToLayer("Furniture Grid");
        filter.mesh = mesh.Build(size, cellSize);
    }

    public Vector2 FromWorldspace(Vector3 point)
    {
        Vector3 local = transform.InverseTransformPoint(point);
        return FromLocalspace(local);
    }

    public Vector2 FromLocalspace(Vector3 point)
    {
        return new Vector2(
            point.x / cellSize + size.x * .5f,
            point.z / cellSize + size.y * .5f
        );
    }

    public Vector3 ToLocalspace(Vector2 coordinates)
    {
        return new Vector3(
            (coordinates.x - size.x * .5f) * cellSize,
            0,
            (coordinates.y - size.y * .5f) * cellSize
        );
    }

    public Vector3 ToWorldspace(Vector2 coordinates)
    {
        Vector3 local = ToLocalspace(coordinates);
        return transform.TransformPoint(local);
    }

    public void AddItem(FurnitureItem item)
    {
        items.Add(item);

        // Get all occupied positions
        Vector2Int[] occupied = items.SelectMany(i => i.region.ToArray()).ToArray();
        filter.mesh = mesh.SetColors(Color.red, occupied); // Rebuild grid mesh with new red positions
    }

    public bool Intersects(int x, int y)
    {
        return Intersects(new Region().FromPoint(x, y));
    }

    public bool Intersects(Region region)
    {
        return items.Any(i => (i.region).Intersect(region).hit);
    }
}
