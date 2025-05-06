using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RoomGenerator
{
    [SerializeField] private float seedDistance = 2;
    [SerializeField] private int growAmount = 4;
    [SerializeField] private int minRoom = 4;
    [SerializeField] private int maxRoom = 9;

    private List<LevelRoom> rooms;
    private Vector2Int size;

    public LevelRoom[] Generate(Vector2Int size)
    {
        this.size = size;

        // Initialize rooms with start and destination rooms
        rooms = new List<LevelRoom>
        {
            new LevelRoom().FromSize(0, 0, minRoom, minRoom),
            new LevelRoom().FromSize(size.x - minRoom, size.y - minRoom, minRoom, minRoom)
        };

        SeedRooms();
        Shuffled().ForEach(GrowRoom);
        Shuffled().ForEach(SplitOversized);
        DiscardUndersized();
        Shuffled().ForEach(GrowRoom);
        Shuffled().ForEach(SplitOversized);
        
        return rooms.ToArray();
    }

    private IEnumerator<LevelRoom> Shuffled()
    {
        List<LevelRoom> rooms = this.rooms.ToList();
        while (rooms.Count > 0)
        {
            int index = Random.Range(0, rooms.Count);
            LevelRoom room = rooms[index];
            rooms.RemoveAt(index);
            yield return room;
        }
    }

    private void DiscardUndersized()
    {
        rooms = rooms.Where((b) => b.size.x >= minRoom && b.size.y >= minRoom).ToList();
    }

    private void GrowRoom(LevelRoom room)
    {
        room.Grow(rooms, growAmount);
        room.Clamp(size);
    }

    private void SplitOversized(LevelRoom room)
    {
        if (room.size.x > maxRoom)
        {
            int cut = Random.Range(room.left + minRoom, room.right + 1 - minRoom);
            LevelRoom newbox = new LevelRoom().FromBounds(cut, room.top, room.right, room.bottom);
            rooms.Add(newbox);
            SplitOversized(newbox);

            room.right = cut;
            SplitOversized(room);
        }
        else if (room.size.y > maxRoom)
        {
            int cut = Random.Range(room.bottom + minRoom, room.top + 1 - minRoom);
            LevelRoom newbox = new LevelRoom().FromBounds(room.left, room.top, room.right, cut);
            rooms.Add(newbox);
            SplitOversized(newbox);

            room.top = cut;
            SplitOversized(room);
        }
    }

    private void SeedRooms()
    {
        for (int i = 0; i < size.x * size.y; i++)
        {
            LevelRoom box = new LevelRoom().FromPoint(Random.Range(0, size.x), Random.Range(0, size.y));

            float separation = rooms.Select((b) => box.Intersect(b).separation).Min();
            if (separation < seedDistance) continue;

            rooms.Add(box);
        }
    }
}
