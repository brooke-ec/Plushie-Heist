using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region : IEnumerable<Vector2Int>
{
    public int top, right, bottom, left;

    public Vector2 center => new Vector2(right + left, top + bottom) * 0.5f;
    public Vector2Int size => new Vector2Int(right - left, top - bottom);
    public Vector2 half => new Vector2(size.x, size.y) * 0.5f;
    
    public Vector2Int position { 
        get { return new Vector2Int(left, bottom); } 
        set {
            top = value.y + size.y;
            right = value.x + size.x;
            bottom = value.y;
            left = value.x;
        } }

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

        IntersectResult result = new IntersectResult(px > 0 && py > 0);

        if (px < py)
        {
            result.dirX = (int)Mathf.Sign(dx);
            result.deltaX = (int)px * result.dirX;
            result.separation = (int)-px;
        }
        else
        {
            result.dirY = (int)Mathf.Sign(dy);
            result.deltaY = (int)py * result.dirY;
            result.separation = (int)-py;
        }

        return result;
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

    public static Region operator *(Region region, int scale)
    {
        return new Region
        {
            top = region.top * scale,
            right = region.right * scale,
            bottom = region.bottom * scale,
            left = region.left * scale
        };
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

    public static T FromSize<T>(this T region, Vector2Int position, Vector2Int size) where T : Region
    {
        return FromSize(region, position.x, position.y, size.x, size.y);
    }

    public static T FromPoint<T>(this T region, int x, int y) where T : Region
    {
        return FromSize(region, x, y, 1, 1);
    }

    public static T FromPoint<T>(this T region, Vector2Int point) where T : Region
    {
        return FromSize(region, point.x, point.y, 1, 1);
    }
}