using UnityEngine;

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
