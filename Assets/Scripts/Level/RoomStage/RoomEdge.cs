using UnityEngine;

public class RoomEdge : Region
{
    public bool connected;
    public LevelRoom roomA;
    public LevelRoom roomB;

    public LevelRoom GetOther(LevelRoom self)
    {
        if (self == roomA) return roomB;
        if (self == roomB) return roomA;
        else throw new System.Exception("Room not in Edge");
    }

    public void Collapse(int width)
    {
        if (right - left >= width)
        {
            left = Random.Range(left, right - width + 1);
            right = left + width;
        }

        if (top - bottom >= width)
        {
            bottom = Random.Range(bottom, top - width + 1);
            top = bottom + width;
        }
    }
}
