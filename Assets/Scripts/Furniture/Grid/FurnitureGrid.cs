using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class FurnitureGrid : MonoBehaviour
{
    public readonly UnityEvent onChanged = new UnityEvent();
    public Vector2Int size => new Vector2Int((int)(collider.size.x / cellSize.x), (int)(collider.size.z / cellSize.y));
    
    private Vector2 cellSize => FurnitureSettings.instance.cellSize * new Vector2(transform.lossyScale.x, transform.lossyScale.z).Reciprocal();
    private static float spacing => FurnitureSettings.instance.spacing;
    private List<FurnitureController> items = new List<FurnitureController>();
    private GridMesh mesh = new GridMesh(Color.green);
    new private BoxCollider collider;
    private MeshFilter filter;

    private void Start()
    {
        if (ShopManager.instance == null) Destroy(gameObject);
        else
        {
            collider = GetComponent<BoxCollider>();
            filter = GetComponent<MeshFilter>();

            gameObject.layer = LayerMask.NameToLayer("Furniture Grid");

            GetComponent<MeshRenderer>().material = FurnitureSettings.instance.gridMaterial;
            filter.mesh = mesh.Build(size, cellSize, spacing);
        }
    }

    public Vector2 FromWorldspace(Vector3 point)
    {
        Vector3 local = transform.InverseTransformPoint(point);
        return FromLocalspace(local);
    }

    public Vector2 FromLocalspace(Vector3 point)
    {
        return new Vector2(
            point.x / cellSize.x + size.x * .5f,
            point.z / cellSize.y + size.y * .5f
        );
    }

    public Vector3 ToLocalspace(Vector2 coordinates)
    {
        return new Vector3(
            (coordinates.x - size.x * .5f) * cellSize.x,
            0,
            (coordinates.y - size.y * .5f) * cellSize.y
        );
    }

    public Vector3 ToWorldspace(Vector2 coordinates)
    {
        Vector3 local = ToLocalspace(coordinates);
        return transform.TransformPoint(local);
    }

    public bool IsEmpty()
    {
        return items.Count == 0;
    }

    public void AddItem(FurnitureController item)
    {
        items.Add(item);
        Regenerate();
        onChanged.Invoke();
        ShopManager.instance.stocksController.TryAddFurnitureToPricingTable(item.item);
    }

    public void RemoveItem(FurnitureController item)
    {
        items.Remove(item);
        Regenerate();
        onChanged.Invoke();
        ShopManager.instance.stocksController.TryRemoveFurnitureFromPricingTable(item.item);
    }

    public void Regenerate()
    {
        // Get all occupied positions
        Vector2Int[] occupied = items.SelectMany(i => i.gridRegion.ToArray()).ToArray();
        mesh.SetColor(Color.green); // Reset grid mesh to green
        filter.mesh = mesh.SetColors(Color.red, occupied); // Rebuild grid mesh with new red positions
    }

    public bool Intersects(int x, int y)
    {
        return Intersects(new Region().FromPoint(x, y));
    }

    public bool Intersects(Region region)
    {
        return items.Any(i => (i.gridRegion).Intersect(region).hit);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (collider.center != Vector3.zero)
        {
            transform.position += collider.center;
            collider.center = Vector3.zero;
        }

        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawMesh(
            mesh.Build(size,Vector2.one * FurnitureSettings.instance.cellSize,spacing),
            transform.TransformPoint(Vector3.zero),
            transform.rotation
        );
    }

    private void OnValidate()
    {
        if (collider == null) collider = GetComponent<BoxCollider>();
    }
#endif
}
