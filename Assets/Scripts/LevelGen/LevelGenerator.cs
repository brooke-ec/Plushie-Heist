using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private LevelTile startTile;
    [SerializeField] private int minRoom = 4;
    [SerializeField] private int maxRoom = 9;
    [SerializeField] private int growAmount = 4;
    [SerializeField] private float tileSize = 5;
    [SerializeField] private float seedDistance = 2;
    [SerializeField] private Vector2Int size = new(24, 24);

    private List<Box> rooms = new();

    private void Start()
    {
        rooms.Add(Box.FromSize(0, 0, minRoom, minRoom));
        rooms.Add(Box.FromSize(size.x - minRoom, size.y - minRoom, minRoom, minRoom));

        Seed();
        
        rooms.Reverse();

        foreach (var box in rooms)
        {
            box.Grow(rooms, growAmount);
            box.Clamp(size);
        }

        int length = rooms.Count;
        for (int i = 0; i < length; i++) Split(rooms[i]);

        rooms = rooms.Where((b) => b.size.x >= minRoom && b.size.y >= minRoom).ToList();
        rooms.Reverse();

        foreach (var box in rooms)
        {
            box.Grow(rooms, growAmount);
            box.Clamp(size);
        }

        length = rooms.Count;
        for (int i = 0; i < length; i++) Split(rooms[i]);

        foreach (var box in rooms) box.Shrink();
        Display(0);
    }

    private void Split(Box box)
    {
        if (box.size.x > maxRoom)
        {
            int cut = Random.Range(box.left + minRoom, box.right + 1 - minRoom);
            Box newbox = Box.FromBounds(cut, box.top, box.right, box.bottom);
            rooms.Add(newbox);
            Split(newbox);

            box.right = cut;
            Split(box);
        }
        else if (box.size.y > maxRoom)
        {
            int cut = Random.Range(box.bottom + minRoom, box.top + 1 - minRoom);
            Box newbox = Box.FromBounds(box.left, box.top, box.right, cut);
            rooms.Add(newbox);
            Split(newbox);

            box.top = cut;
            Split(box);
        }

    }

    private void Seed()
    {
        for (int i = 0; i < size.x * size.y; i++)
        {
            Box box = Box.FromPoint(Random.Range(0, size.x), Random.Range(0, size.y));

            float separation = rooms.Select((b) => box.Intersects(b).separation).Min();
            if (separation < seedDistance) continue;

            rooms.Add(box);
        }
    }

    private void Display(float y)
    {
        foreach (Box box in rooms)
            for (int x = box.left; x < box.right; x++)
                for (int z = box.bottom; z < box.top; z++)
                    Instantiate(startTile, new Vector3(x, y, z) * tileSize, Quaternion.identity, transform);
    }

    private class Box
    {
        public int top, right, bottom, left;

        public Vector2 center => new Vector2(right + left, top + bottom) * 0.5f;
        public Vector2Int size => new Vector2Int(right - left, top - bottom);
        public Vector2 half => new Vector2(size.x, size.y) * 0.5f;

        private Box(int x1, int y1, int x2, int y2)
        {
            this.top = Mathf.Max(y1, y2);
            this.right = Mathf.Max(x1, x2);
            this.bottom = Mathf.Min(y1, y2);
            this.left = Mathf.Min(x1, x2);
        }

        public static Box FromBounds(int x1, int y1, int x2, int y2)
        {
            return new Box(x1, y1, x2, y2);
        }

        public static Box FromSize(int x, int y, int width, int height)
        {
            return new Box(x, y, x + width, y + height);
        }

        public static Box FromPoint(int x, int y)
        {
            return Box.FromSize(x, y, 1, 1);
        }

        public IntersectHit Intersects(Box other)
        {
            float dx = other.center.x - this.center.x;
            float px = (other.half.x + this.half.x) - Mathf.Abs(dx);

            float dy = other.center.y - this.center.y;
            float py = (other.half.y + this.half.y) - Mathf.Abs(dy);

            IntersectHit hit = new IntersectHit(px > 0 && py > 0);

            if (px < py)
            {
                hit.dirX = (int) Mathf.Sign(dx);
                hit.deltaX = (int) px * hit.dirX;
                hit.separation = (int) -px;
            } else
            {
                hit.dirY = (int) Mathf.Sign(dy);
                hit.deltaY = (int) py * hit.dirY;
                hit.separation = (int) -py;
            }

            return hit;
        }

        public void Grow(List<Box> boxes, int maxGrow)
        {
            int up = maxGrow;
            int right = maxGrow;
            int down = -maxGrow;
            int left = -maxGrow;

            foreach (Box box in boxes)
            {
                if (box == this) continue;

                IntersectHit hit = this.Intersects(box);

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

        public void Shrink()
        {
            left += 1;
            bottom += 1;
        }
    }

    private struct IntersectHit
    {
        public int deltaX, deltaY;
        public int dirX, dirY;
        public int separation;
        public bool hit;

        public readonly Vector2Int delta => new Vector2Int(deltaX, deltaY);

        public IntersectHit(bool hit)
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
