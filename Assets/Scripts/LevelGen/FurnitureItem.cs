using UnityEngine;

public class FurnitureItem : MonoBehaviour
{
    public Vector2Int size;

    public void Rotate()
    {
        size = new Vector2Int(size.y, size.x);
        transform.Rotate(0, 90, 0);
    }
}
