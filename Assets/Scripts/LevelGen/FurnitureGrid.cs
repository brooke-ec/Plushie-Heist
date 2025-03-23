using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(BoxCollider))]
public class FurnitureGrid : MonoBehaviour
{
    [SerializeField] private float cellSize = 2;
    [SerializeField] private float spacing = 0.25f;

    private Vector2Int size { get { return new Vector2Int((int)(collider.size.x / cellSize), (int)(collider.size.z / cellSize)); } }
    private float height { get { return collider.bounds.max.y; } }

    new private BoxCollider collider;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        collider = GetComponent<BoxCollider>();
        Gizmos.DrawWireCube(transform.TransformPoint(collider.center), collider.size);
    }

    private void Start()
    {
        collider = GetComponent<BoxCollider>();
        GetComponent<MeshFilter>().mesh = Build();
    }

    private Mesh Build()
    {
        List<Vector3> verticies = new List<Vector3>();
        List<int> indicies = new List<int>();

        float far = cellSize - spacing;

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                float xc = (x - size.x / 2f) * cellSize;
                float yc = (y - size.y / 2f) * cellSize;

                indicies.AddRange(new int[] {
                    0, 1, 2,
                    1, 3, 2
                }.Select(i => i + verticies.Count));

                verticies.Add(new Vector3(xc + spacing, height, yc + spacing));
                verticies.Add(new Vector3(xc + spacing, height, yc + far));
                verticies.Add(new Vector3(xc + far, height, yc + spacing));
                verticies.Add(new Vector3(xc + far, height, yc + far));
            }

        Mesh mesh = new Mesh();
        mesh.SetVertices(verticies);
        mesh.SetIndices(indicies, MeshTopology.Triangles, 0);
        return mesh;
    }
}
