using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridMesh
{
    public Vector2Int size { get; private set; }
    public Mesh mesh { get; private set; }
    public float cellSize { get; private set; }
    public float spacing { get; private set; }
    public Color color { get; private set; }

    public GridMesh(Color color, float spacing)
    {
        this.color = color;
        this.spacing = spacing;
    }

    public GridMesh(Color color) : this(color, 0.3f) { }

    public GridMesh() : this(Color.blue) { }

    public Mesh Build(Vector2Int size, float cellSize)
    {
        return Build(size, cellSize, spacing);
    }

    public Mesh Build(Vector2Int size, float cellSize, float spacing)
    {
        this.cellSize = cellSize;
        this.spacing = spacing;
        this.size = size;

        // Initialise constants
        List<Vector3> verticies = new List<Vector3>();
        List<int> indicies = new List<int>();

        float gap = cellSize * spacing / 2;
        float far = cellSize - gap;

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                Vector3 l = new Vector3(
                    (x - size.x * .5f) * cellSize,
                    0,
                    (y - size.y * .5f) * cellSize
                );

                indicies.AddRange(new int[] {
                    0, 1, 2,
                    1, 3, 2
                }.Select(i => i + verticies.Count));

                verticies.Add(new Vector3(l.x + gap, 0, l.z + gap));
                verticies.Add(new Vector3(l.x + gap, 0, l.z + far));
                verticies.Add(new Vector3(l.x + far, 0, l.z + gap));
                verticies.Add(new Vector3(l.x + far, 0, l.z + far));
            }

        mesh = new Mesh();
        mesh.SetVertices(verticies);
        mesh.SetIndices(indicies, MeshTopology.Triangles, 0);
        mesh.SetNormals(verticies.Select(_ => new Vector3(0, 1, 0)).ToArray());
        mesh.SetColors(verticies.Select(_ => color).ToArray());

        return mesh;
    }

    public Mesh SetColors(Color color, Vector2Int[] positions)
    {
        Color[] colors = mesh.colors;
        positions.ForEach(position =>
        {
            int start = (position.x * size.y + position.y) * 4;
            for (int i = 0; i < 4; i++) colors[start + i] = color;
        });

        mesh.SetColors(colors);
        return mesh;
    }

    public Mesh SetColor(Color color, Vector2Int position)
    {
        return SetColors(color, new Vector2Int[] { position });
    }

    public Mesh SetColor(Color color)
    {
        this.color = color;
        mesh.SetColors(mesh.vertices.Select(_ => color).ToArray());
        return mesh;
    }
}
