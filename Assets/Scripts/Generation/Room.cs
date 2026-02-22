using JetBrains.Annotations;

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

    [CanBeNull] private Connectable left;
    [CanBeNull] private Connectable up;
    [CanBeNull] private Connectable right;
    [CanBeNull] private Connectable down;

    public Connectable? Left
    {
        get { return left; }
        set { left = value; }
    }
    
    public Connectable? Up
    {
        get { return up; }
        set { up = value; }
    }
    
    public Connectable? Right
    {
        get { return right; }
        set { right = value; }
    }
    
    public Connectable? Down
    {
        get { return down; }
        set { down = value; }
    }

    public int MaxConnections
    {
        get { return maxConnections; }
        set { maxConnections = value; }
    }

    public bool IsOrigin { get; set; } = false;
    public bool IsEndRoom { get; set; } = false;
    public bool IsSpecial { get; set; } = false;

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

        if (IsSpecial)
        {
            return "S";
        }

        return "R";
    }
}
