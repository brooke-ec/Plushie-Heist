using UnityEngine;

public abstract class GridTransformations: MonoBehaviour
{
    public abstract Vector2Int size { get; }
    public abstract Vector2 cellSize { get; }

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
}
