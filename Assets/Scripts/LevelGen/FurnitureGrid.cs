using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(BoxCollider))]
public class FurnitureGrid : MonoBehaviour
{
    [SerializeField] private float cellSize = 2;
    [SerializeField] private float spacing = 0.25f;

    [HideInInspector] public Vector2Int size => new Vector2Int((int)(collider.size.x / cellSize), (int)(collider.size.z / cellSize));
    private float height => collider.center.y + collider.size.y * .5f;

    private List<FurnitureItem> items = new List<FurnitureItem>();
    new private BoxCollider collider;

#if UNITY_EDITOR
    private Vector3 oldSize = Vector3.zero;
    private Mesh mesh;

    private void OnDrawGizmos()
    {
        if (collider.size != oldSize || mesh == null) mesh = BuildMarker();

        Gizmos.color = Color.blue;
        Gizmos.DrawWireMesh(mesh, transform.TransformPoint(collider.center));
    }

    private void OnValidate()
    {
        if (collider == null) collider = GetComponent<BoxCollider>();
        mesh = null;
    }
#endif

    private void Start()
    {
        collider = GetComponent<BoxCollider>();
        GetComponent<MeshFilter>().mesh = BuildMarker();
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
            height,
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
    }

    public bool Intersects(Region region)
    {
        return items.Any(r => r.region.Intersect(region).hit);
    }

    private Mesh BuildMarker()
    {
        List<Vector3> verticies = new List<Vector3>();
        List<int> indicies = new List<int>();

        float far = cellSize - spacing;

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                Vector3 l = ToLocalspace(new Vector2(x, y));

                indicies.AddRange(new int[] {
                    0, 1, 2,
                    1, 3, 2
                }.Select(i => i + verticies.Count));

                verticies.Add(new Vector3(l.x + spacing, height, l.z + spacing));
                verticies.Add(new Vector3(l.x + spacing, height, l.z + far));
                verticies.Add(new Vector3(l.x + far, height, l.z + spacing));
                verticies.Add(new Vector3(l.x + far, height, l.z + far));
            }

        Mesh mesh = new Mesh();
        mesh.SetVertices(verticies);
        mesh.SetIndices(indicies, MeshTopology.Triangles, 0);
        mesh.SetNormals(verticies.Select(_ => new Vector3(0, 1, 0)).ToArray());
        return mesh;
    }
}
