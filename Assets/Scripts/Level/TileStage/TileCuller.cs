using UnityEngine;

public class TileCuller : MonoBehaviour
{
    private bool[,,,] visionMatrix;
    private State[,,,] workingMatrix;
    private LevelTile[] tiles;

    private TileGenerator tileGenerator;
    private Transform player;
    private Vector2Int previous;

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>().transform;
        tileGenerator = GetComponent<LevelGenerator>().tileGenerator;
    }

    void Update()
    {
        Vector2Int position = Vector2Int.FloorToInt(FromWorldspace(player.position));
        if (previous != position)
        {
            tiles.ForEach(t => t.gameObject.SetActive(visionMatrix[position.x, position.y, t.position.x, t.position.y]));
            previous = position;
        }
    }

    public Vector2 FromWorldspace(Vector3 point)
    {
        Vector3 local = transform.InverseTransformPoint(point);
        Vector3 grid = (local + tileGenerator.physicalOffset) / tileGenerator.tileSize;
        return new Vector2(grid.x, grid.z) + Vector2.one * .5f;
    }

    public void Setup(bool[,] spaces, LevelTile[] tiles)
    {
        this.tiles = tiles; 

        workingMatrix = new State[spaces.GetLength(0), spaces.GetLength(1), spaces.GetLength(0), spaces.GetLength(1)];
        visionMatrix = new bool[spaces.GetLength(0), spaces.GetLength(1), spaces.GetLength(0), spaces.GetLength(1)];

        // This is not good coding practice, but its funny and works (hopefully)
        for (int x1 = 0; x1 < spaces.GetLength(0); x1++)
        {
            for (int y1 = 0; y1 < spaces.GetLength(1); y1++)
            {
                for (int x2 = 0; x2 < spaces.GetLength(0); x2++)
                {
                    for (int y2 = 0; y2 < spaces.GetLength(1); y2++)
                    {
                        if (workingMatrix[x1, y1, x2, y2] != State.Unknown) continue;
                        State state = CheckVisibility(new Vector2Int(x1, y1), new Vector2Int(x2, y2), spaces);
                        visionMatrix[x1, y1, x2, y2] = state == State.Visible;
                        visionMatrix[x2, y2, x1, y1] = state == State.Visible;
                        workingMatrix[x1, y1, x2, y2] = state;
                        workingMatrix[x2, y2, x1, y1] = state;
                    }
                }
            }
        }

        workingMatrix = null;
    }

    private State CheckVisibility(Vector2Int a, Vector2Int b, bool[,] spaces)
    {
        if (a == b) return State.Visible;
        float gradient = float.NegativeInfinity;
        int connections = 0;

        for (int x1o = 0; x1o <= 1; x1o++)
        {
            for (int y1o = 0; y1o <= 1; y1o++)
            {
                for (int x2o = 0; x2o <= 1; x2o++)
                {
                    for (int y2o = 0; y2o <= 1; y2o++)
                    {
                        Vector2 c = new Vector2(a.x + x1o, a.y + y1o);
                        Vector2 d = new Vector2(b.x + x2o, b.y + y2o);
                        if (CheckLine(c, d, a, spaces))
                        {
                            connections++;
                            float newGradient = float.PositiveInfinity;
                            float dy = b.y - a.y;
                            float dx = b.x - a.x;

                            if (dx != 0) newGradient = dy / dx;
                            if (newGradient != gradient)
                            {
                                gradient = newGradient;
                                connections++;

                                if (connections == 2) return State.Visible;
                            }
                        }
                    }
                }
            }
        }

        return State.Hidden;
    }

    private bool CheckLine(Vector2 a, Vector2 b, Vector2Int cell, bool[,] spaces)
    {
        if (a == b) return true;
        Vector2 c = cell;

        Vector2 center = cell + Vector2.one * 0.5f;
        bool horizontal = Mathf.Sign(a.x - center.x) == Mathf.Sign(b.x - center.x);
        bool vertical = Mathf.Sign(a.y - center.y) == Mathf.Sign(b.y - center.y);
        
        bool skipFirst = !(horizontal && vertical);
        bool skipDiagonal = horizontal != vertical;

        bool visible = true;

        while (true)
        {
            Vector2 direction = b - a;
            Vector2 reflection = new Vector2(Mathf.Sign(direction.x), Mathf.Sign(direction.y));
            Vector2 aRelative = new Vector2(a.x - c.x, (a.y - c.y) * reflection.y);
            Vector2 dRelative = new Vector2(direction.x, direction.y * reflection.y);

            float u = Mathf.Max(0, reflection.y) - aRelative.y;
            if (dRelative.y == 0) u = 5 * reflection.x;
            else u = aRelative.x + u / dRelative.y * dRelative.x;

            bool diagonal = false;
            u = u * reflection.x - Mathf.Max(0, reflection.x);
            if (u >= 0) c.x += reflection.x;
            if (u <= 0) c.y += reflection.y;
            if (u == 0) diagonal = true;

            Vector2 length = new Vector2(
                c.x - a.x - Mathf.Min(reflection.x, 0),
                c.y - a.y - Mathf.Min(reflection.y, 0) 
            );

            if (Mathf.Abs(length.x) >= Mathf.Abs(direction.x) && direction.x != 0 || Mathf.Abs(length.y) >= Mathf.Abs(direction.y) && direction.y != 0) break;
            
            if (!skipFirst)
            {
                Vector2Int position = Vector2Int.FloorToInt(c);
                if (position.x >= spaces.GetLength(0) || position.y >= spaces.GetLength(1)) visible &= true;
                else visible &= spaces[position.x, position.y];

                if (!skipDiagonal) visible &= !diagonal || spaces[position.x, position.y - Mathf.RoundToInt(reflection.y)]
                    || spaces[position.x - Mathf.RoundToInt(reflection.x), position.y];
            }
            skipDiagonal = false;
            skipFirst = false;
        }

        return visible;
    }

    private enum State
    {
        Unknown,
        Visible,
        Hidden,
    }
}
