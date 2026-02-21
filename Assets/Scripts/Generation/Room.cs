using UnityEngine;

public class Room : Connectable
{
    public enum ConnectionDirection
    {
        Left,
        Up,
        Right,
        Down
    }

    private int maxConnections;

    public Connectable? Left { get; set; } = null;
    public Connectable? Right { get; set; } = null;
    public Connectable? Up { get; set; } = null;
    public Connectable? Down { get; set; } = null;

    public int MaxConnections
    {
        get { return maxConnections; }
        set { maxConnections = value; }
    }

    public bool IsOrigin { get; private set; } = false;
    public bool IsEndRoom { get; set; } = false;

    public Room()
    {
        Connections = new[] { Left, Up, Right, Down };
        maxConnections = 4;
    }

    public Room(int maxConnections)
    {
        Connections = new[] { Left, Up, Right, Down };
        MaxConnections = maxConnections;
    }

    public Room(bool isOrigin)
    {
        Connections = new[] { Left, Up, Right, Down };
        MaxConnections = 4;
        IsOrigin = isOrigin;
    }

    public Room(int maxConnections, bool isOrigin)
    {
        Connections = new[] { Left, Up, Right, Down };
        MaxConnections = maxConnections;
        IsOrigin = isOrigin;
    }

    public override string ToString()
    {
        if (IsOrigin)
        {
            return "O";
        }

        if (IsEndRoom)
        {
            return "E";
        }

        return "R";
    }
}
