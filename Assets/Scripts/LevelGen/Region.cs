using UnityEngine;

public class Region
{
    public int top, right, bottom, left;

    public Vector2 center => new Vector2(right + left, top + bottom) * 0.5f;
    public Vector2Int size => new Vector2Int(right - left, top - bottom);
    public Vector2 half => new Vector2(size.x, size.y) * 0.5f;

    private Region(int x1, int y1, int x2, int y2)
    {
        this.top = Mathf.Max(y1, y2);
        this.right = Mathf.Max(x1, x2);
        this.bottom = Mathf.Min(y1, y2);
        this.left = Mathf.Min(x1, x2);
    }

    public static Region FromBounds(int x1, int y1, int x2, int y2)
    {
        return new Region(x1, y1, x2, y2);
    }

    public static Region FromSize(int x, int y, int width, int height)
    {
        return new Region(x, y, x + width, y + height);
    }

    public static Region FromPoint(int x, int y)
    {
        return Region.FromSize(x, y, 1, 1);
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

    public class IntersectResult
    {
        public int deltaX, deltaY;
        public int dirX, dirY;
        public int separation;
        public bool hit;

        public Vector2Int delta => new Vector2Int(deltaX, deltaY);

        public IntersectResult(bool hit)
        {
            this.separation = 0;
            this.hit = hit;
            deltaX = 0;
            deltaY = 0;
            dirX = 0;
            dirY = 0;
        }
    }
}
