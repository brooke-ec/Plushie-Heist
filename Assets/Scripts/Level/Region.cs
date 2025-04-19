using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region : IEnumerable<Vector2Int>
{
    public int top, right, bottom, left;

    public Vector2 center => new Vector2(right + left, top + bottom) * 0.5f;
    public Vector2Int size => new Vector2Int(right - left, top - bottom);
    public Vector2 half => new Vector2(size.x, size.y) * 0.5f;
    public Vector2 start => new Vector2(left, bottom);
    public Vector2 end => new Vector2(right, top);

    public bool Within(Vector2Int size)
    {
        return bottom >= 0
            && left >= 0
            && top <= size.y
            && right <= size.x;
    }

    public IntersectResult Intersect(Region other)
    {
        float dx = other.center.x - this.center.x;
        float px = (other.half.x + this.half.x) - Mathf.Abs(dx);

        float dy = other.center.y - this.center.y;
        float py = (other.half.y + this.half.y) - Mathf.Abs(dy);

        IntersectResult hit = new IntersectResult(px > 0 && py > 0);

        if (px < py)
        {
            hit.dirX = (int)Mathf.Sign(dx);
            hit.deltaX = (int)px * hit.dirX;
            hit.separation = (int)-px;
        }
        else
        {
            hit.dirY = (int)Mathf.Sign(dy);
            hit.deltaY = (int)py * hit.dirY;
            hit.separation = (int)-py;
        }

        return hit;
    }

    public IEnumerator<Vector2Int> GetEnumerator()
    {
        for (int x = left; x < right; x++)
            for (int y = bottom; y < top; y++)
                yield return new Vector2Int(x, y);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public static class RegionFactory
{
    public static T FromBounds<T>(this T region, int x1, int y1, int x2, int y2) where T : Region
    {
        region.top = Mathf.Max(y1, y2);
        region.right = Mathf.Max(x1, x2);
        region.bottom = Mathf.Min(y1, y2);
        region.left = Mathf.Min(x1, x2);
        return region;
    }

    public static T FromSize<T>(this T region, int x, int y, int width, int height) where T : Region
    {
        return FromBounds(region, x, y, x + width, y + height);
    }

    public static T FromPoint<T>(this T region, int x, int y) where T : Region
    {
        return FromSize(region, x, y, 1, 1);
    }
}