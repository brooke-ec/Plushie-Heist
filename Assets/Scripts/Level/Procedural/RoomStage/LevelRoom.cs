using System.Collections.Generic;
using UnityEngine;

public class LevelRoom : Region
{
    public List<RoomEdge> edges = new List<RoomEdge>();
    public int pathDistance = -1;
    public int distance = -1;
    public RoomEdge backtrack;

    public void Grow(List<LevelRoom> boxes, int maxGrow)
    {
        int up = maxGrow;
        int right = maxGrow;
        int down = -maxGrow;
        int left = -maxGrow;

        foreach (LevelRoom box in boxes)
        {
            if (box == this) continue;

            IntersectResult hit = Intersect(box);

            if (hit.hit) Debug.LogError("Overlapping boxes! D:");

            if (hit.dirX > 0) right = Mathf.Min(right, -hit.deltaX);
            else if (hit.dirX < 0) left = Mathf.Max(left, -hit.deltaX);

            if (hit.dirY > 0) up = Mathf.Min(up, -hit.deltaY);
            else if (hit.dirY < 0) down = Mathf.Max(down, -hit.deltaY);
        }

        this.top += up;
        this.right += right;
        this.bottom += down;
        this.left += left;
    }

    public void Clamp(Vector2Int size)
    {
        top = Mathf.Min(top, size.y);
        right = Mathf.Min(right, size.x);
        bottom = Mathf.Max(bottom, 0);
        left = Mathf.Max(left, 0);
    }

    public void Shrink(int amount)
    {
        left += amount;
        bottom += amount;
    }
}
