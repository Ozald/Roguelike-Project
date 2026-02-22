using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;
using Vector2 = UnityEngine.Vector2;

public class TileGraph : MonoBehaviour
{
    public Connectable roomPrefab;
    public Connectable hallPrefab;
    
    public enum Half
    {
        Left,
        Right,
        Top,
        Bottom
    }

    private int width;
    private int height;
    private Connectable[,] grid;
    private int maxRoomsPerBranch;
    private readonly Random random = new();
    private readonly Dictionary<Room, Vector2> rooms;
    private readonly Dictionary<Hallway, Vector2> halls;
    private Vector2 startPos;
    public Room? Start;
    public float extraHallsChance;
    public int specialRooms;
    public float specialRoomsChance;
    
    public float ExtraHallsChance { get => extraHallsChance; set => extraHallsChance = value; }

    public int Width
    {
        get { return width; }
        set
        {
            if (value < 3)
            {
                throw new ArgumentException("Width must be >= 3.");
            }

            width = value;
        }
    }

    public int Height
    {
        get { return height; }
        set
        {
            if (value < 3)
            {
                throw new ArgumentException("Height must be >= 3");
            }

            height = value;
        }
    }

    public int RoomCount
    {
        get { return rooms.Count; }
    }

    public int HallCount
    {
        get { return halls.Count; }
    }

    public Vector2 StartPosition
    {
        get { return startPos; }
        private set { startPos = value; }
    }

    public int MaxRoomsPerBranch
    {
        get { return maxRoomsPerBranch; }
        set { maxRoomsPerBranch = value; }
    }

    public Connectable[,] Grid
    {
        get { return grid; }
        private set { grid = value; }
    }

    public Room? StartRoom
    {
        get { return Start; }
        set { Start = value; }
    }

    public TileGraph(int width, int height, int maxRoomsPerBranch)
    {
        this.width = width;
        this.height = height;
        this.maxRoomsPerBranch = maxRoomsPerBranch;
        rooms = new();
        halls = new();
        startPos = new Vector2(-1, -1);

        grid = new Connectable[width, height];
    }

    // Depth-first generation
    public void GenerateMap(Vector2 startPosition)
    {
        if (!InBounds(startPosition))
            throw new ArgumentException("Start position is not in the map.");

        // Create the starting room and start the recursive generation
        // Rule: The origin room has 4 possible branches
        Room startRoom = Instantiate(roomPrefab.gameObject, 
            new Vector3(startPosition.x * 5, startPosition.y * 5, 0), Quaternion.identity).GetComponent<Room>();
        StartPosition = startPosition;
        startRoom.IsOrigin = true;
        Start = startRoom;
        
        startRoom.gameObject.GetComponent<Renderer>().material.color = Color.green;

        GenerateFrom(startRoom, startPosition, 0, (int)(MaxRoomsPerBranch * 0.75));
        AddHalls(extraHallsChance);
        SetEndRoom();
        SetSpecialRooms();
    }

    // Recursive helper for standard depth-first generation
    private bool GenerateFrom(Room start, Vector2 startVector, int roomsGenerated, int penaltySafety)
    {
        if (roomsGenerated >= maxRoomsPerBranch)
            return false;

        if (!InBounds(startVector))
            return false;

        // Prevent generating rooms on top of each other.
        // This should also stop hallways from overwriting things.
        // This was really only here because the origin room kept
        // getting overwritten.
        if (!IsSpotEmpty(startVector))
            return false;

        if (start is null)
        {
            Debug.LogError("Start room is null.");
            return false;
        }
        
        if (!rooms.ContainsKey(start))
        {
            PlaceAt(start, startVector);
            rooms.Add(start, startVector);
        }

        // Randomize the order that we generate the directions in
        PriorityQueue<Room.ConnectionDirection> connections = new();

        foreach (Room.ConnectionDirection direction in Enum.GetValues(typeof(Room.ConnectionDirection)))
        {
            // Attempt to bring some balance to the distribution of the rooms
            Half leastPopulated = LeastPopulatedHalf();
            float multiplier = 1 - roomsGenerated * 0.03f * (float)random.NextDouble();

            if (leastPopulated == Half.Top && direction == Room.ConnectionDirection.Up)
                connections.Enqueue(direction, (int)(random.Next(4) * multiplier * 100));
            else if (leastPopulated == Half.Bottom && direction == Room.ConnectionDirection.Down)
                connections.Enqueue(direction, (int)(random.Next(4) * multiplier * 100));
            else if (leastPopulated == Half.Left && direction == Room.ConnectionDirection.Left)
                connections.Enqueue(direction, (int)(random.Next(4) * multiplier * 100));
            else if (leastPopulated == Half.Right && direction == Room.ConnectionDirection.Right)
                connections.Enqueue(direction, (int)(random.Next(4) * multiplier * 100));
            else
                connections.Enqueue(direction, random.Next(4));
        }

        // Add some connections, maybe not all
        int connectionCount = 0;
        int usedConnections = (int)Math.Ceiling(start.MaxConnections *
                                                (1d - (double)roomsGenerated / maxRoomsPerBranch)) + random.Next(-1, 1);

        // Heuristic to decrease the amount of depth we can get from the recursion
        if (roomsGenerated > penaltySafety)
        {
            float depthPenalty = CalculateDepthPenalty(roomsGenerated);
            usedConnections = (int)Math.Floor(usedConnections * depthPenalty);
        }

        // Generate the branching halls and rooms
        while (connectionCount < usedConnections && connections.Count > 0)
        {
            Room.ConnectionDirection direction = connections.Dequeue();

            switch (direction)
            {
                case Room.ConnectionDirection.Left:
                    //random.Next(roomsGenerated < maxRoomsPerBranch / 4 ? 2 : 1, 5)
                    start.Left = Instantiate(roomPrefab.gameObject, 
                        new Vector3(startVector.x * 5, startVector.y * 5, 0), Quaternion.identity).GetComponent<Room>();

                    if (roomsGenerated < maxRoomsPerBranch - 1
                        && GenerateFrom((Room)start.Left, startVector + new Vector2(-2, 0), roomsGenerated + 1,
                            penaltySafety))
                    {
                        Console.WriteLine("Generating left branch");
                        PlaceHallAt(startVector + new Vector2(-1, 0), start, (Room)start.Left);
                        connectionCount++;
                    }
                    else
                    {
                        Destroy(start.Left.gameObject);
                    }

                    break;
                case Room.ConnectionDirection.Right:
                    start.Right = Instantiate(roomPrefab.gameObject, 
                        new Vector3(startVector.x * 5, startVector.y * 5, 0), Quaternion.identity).GetComponent<Room>();

                    if (roomsGenerated < maxRoomsPerBranch - 1
                        && GenerateFrom((Room)start.Right, startVector + new Vector2(2, 0), roomsGenerated + 1,
                            penaltySafety))
                    {
                        Console.WriteLine("Generating right branch");
                        PlaceHallAt(startVector + new Vector2(1, 0), start, (Room)start.Right);
                        connectionCount++;
                    }
                    else
                    {
                        Destroy(start.Right.gameObject);
                    }

                    break;
                case Room.ConnectionDirection.Up:
                    start.Up = Instantiate(roomPrefab.gameObject, 
                        new Vector3(startVector.x * 5, startVector.y * 5, 0), Quaternion.identity).GetComponent<Room>();

                    if (roomsGenerated < maxRoomsPerBranch - 1
                        && GenerateFrom((Room)start.Up, startVector + new Vector2(0, -2), roomsGenerated + 1,
                            penaltySafety))
                    {
                        Console.WriteLine("Generating up branch");
                        PlaceHallAt(startVector + new Vector2(0, -1), start, (Room)start.Up);
                        connectionCount++;
                    }
                    else
                    {
                        Destroy(start.Up.gameObject);
                    }

                    break;
                case Room.ConnectionDirection.Down:
                    start.Down = Instantiate(roomPrefab.gameObject, 
                        new Vector3(startVector.x * 5, startVector.y * 5, 0), Quaternion.identity).GetComponent<Room>();

                    if (roomsGenerated < maxRoomsPerBranch - 1
                        && GenerateFrom((Room)start.Down, startVector + new Vector2(0, 2), roomsGenerated + 1,
                            penaltySafety))
                    {
                        Console.WriteLine("Generating down branch");
                        PlaceHallAt(startVector + new Vector2(0, 1), start, (Room)start.Down);
                        connectionCount++;
                    }
                    else
                    {
                        Destroy(start.Down.gameObject);
                    }

                    break;
            }
        }

        return true;
    }

    // Adds extra halls to the map
    private void AddHalls(float chance)
    {
        List<Room> roomList = rooms.Keys.ToList();

        for (int i = 0; i < roomList.Count; i++)
        {
            for (int j = i + 1; j < roomList.Count; j++)
            {
                Room originRoom = roomList[i];
                Room end = roomList[j];

                if (random.NextDouble() < chance)
                {
                    if (Math.Abs(XDist(originRoom, end)) == 2 && YDist(originRoom, end) == 0)
                    {
                        if (XDist(originRoom, end) == -2)
                        {
                            if (PlaceHallAt(rooms[originRoom] + new Vector2(1, 0), originRoom, end))
                                Console.WriteLine("Added extra right hall");
                        }
                        else if (XDist(originRoom, end) == 2)
                        {
                            if (PlaceHallAt(rooms[originRoom] + new Vector2(-1, 0), originRoom, end))
                                Console.WriteLine("Added extra left hall");
                        }
                    }
                    else if (Math.Abs(YDist(originRoom, end)) == 2 && XDist(originRoom, end) == 0)
                    {
                        if (YDist(originRoom, end) == 2)
                        {
                            if (PlaceHallAt(rooms[originRoom] + new Vector2(0, -1), originRoom, end))
                                Console.WriteLine("Added extra up hall");
                        }
                        else if (YDist(originRoom, end) == -2)
                        {
                            if (PlaceHallAt(rooms[originRoom] + new Vector2(0, 1), originRoom, end))
                                Console.WriteLine("Added extra down hall");
                        }
                    }
                }
            }
        }
    }

    /*
    // Generates the map from a random walk.
    // No immediate doubling back is allowed.
    public void GenerateRandomWalk(int maxSteps, int minStepSize, int maxStepSize, double roomGenChance, Vector2 startPos)
    {
        Room start = new(4, true);
        PlaceAt(start, startPos);
        StartPosition = startPos;

        int roomsGenerated = 0;
        int steps = 0;
        Vector2 currentPosition = startPos;

        List<Vector2> directions = [new(1, 0), new(-1, 0), new(0, 1), new(0, -1)];
        Vector2 previousDirection = Vector2.Zero;

        while (steps < maxSteps && roomsGenerated < MaxRoomsPerBranch)
        {
            if (!InBounds(currentPosition))
            {
                break;
            }

            // Generate a room based on chance parameter
            if (random.NextDouble() < roomGenChance)
            {
                if (PlaceAt(new Room(4, false), currentPosition))
                {
                    roomsGenerated++;
                    if (GetAt(currentPosition) != null)
                    {
                        rooms[(Room)GetAt(currentPosition)] = currentPosition;
                    }

                    roomGenChance *= 0.9;
                }
            }

            Vector2 direction = directions[random.Next(directions.Count)];

            while (direction == previousDirection)
            {
                direction = directions[random.Next(directions.Count)];
            }

            int stepSize = random.Next(minStepSize, maxStepSize + 1);

            currentPosition += direction * stepSize;
            steps++;
            previousDirection = direction;
        }

        Console.WriteLine("Took " + steps + " steps and generated " + roomsGenerated + " rooms.");

        ConnectRooms();
        SetEndRoom();
    }

    // Connects rooms together with hallways
    public void ConnectRooms()
    {
        List<Room> connected = [(Room)GetAt(StartPosition)];
        Vector2 currentPos = StartPosition;
        Connectable? previous = GetAt(currentPos);

        while (connected.Count < rooms.Keys.Count + 1)
        {
            Room nearest = FindNearestUnconnected(currentPos, connected);
            Vector2 targetPos = rooms[nearest];

            while (currentPos != targetPos)
            {
                Vector2 step = Vector2.Zero;

                if (random.NextDouble() < 0.5)
                {
                    step.x = Math.Sign(targetPos.x - currentPos.x);
                }
                else
                {
                    step.y = Math.Sign(targetPos.y - currentPos.y);
                }

                currentPos += step;

                if (IsSpotEmpty(currentPos))
                {
                    Hallway hall = new() { Origin = previous };
                    if (previous is Hallway h) h.End = hall;
                    PlaceHallAt(currentPos, previous, hall);
                    previous = hall;
                }
            }

            connected.Add(nearest);
            previous = GetAt(currentPos);
        }
    }

    // Connects rooms together with more linear hallways
    public void ConnectRoomsLinear()
    {
        List<Room> connected = [];

        Vector2 currentPosition = StartPosition;
        int connectedRooms = 0;
        Connectable? previous = GetAt(currentPosition);
        connected.Add((Room)previous);

        while (connectedRooms < rooms.Keys.Count)
        {
            Room nearest = FindNearestUnconnected(currentPosition, connected);
            int xDist = (int)(currentPosition.x - rooms[nearest].x);
            int yDist = (int)(currentPosition.y - rooms[nearest].y);

            //Console.WriteLine(xDist + " " + yDist);

            while (xDist != 0 || yDist != 0)
            {
                xDist = (int)(currentPosition.x - rooms[nearest].x);
                yDist = (int)(currentPosition.y - rooms[nearest].y);

                if (xDist == 0 && yDist == 0)
                {
                    //Console.WriteLine("Connected rooms");
                    break;
                }

                int xOry = -1;

                if (xDist == 0 && yDist != 0)
                {
                    xOry = yDist;
                }
                else if (xDist != 0 && yDist == 0)
                {
                    xOry = xDist;
                }
                else if (xDist != 0 && yDist != 0)
                {
                    xOry = Math.Max(xDist, yDist);
                }

                if (xOry == xDist)
                {
                    while (xDist != 0)
                    {
                        xDist = (int)(currentPosition.x - rooms[nearest].x);

                        if (xDist > 0)
                        {
                            //Console.WriteLine("Going left");
                            currentPosition += new Vector2(-1, 0);
                        }
                        else if (xDist < 0)
                        {
                            //Console.WriteLine("Going right");
                            currentPosition += new Vector2(1, 0);
                        }

                        Hallway hall = new();

                        if (previous != null)
                        {
                            if (previous is Hallway h)
                            {
                                h.End = hall;
                            }

                            hall.Origin = previous;
                        }

                        PlaceHallAt(currentPosition, previous, hall);
                        previous = hall;
                    }
                }
                else
                {
                    while (yDist != 0)
                    {
                        yDist = (int)(currentPosition.y - rooms[nearest].y);
                        if (yDist > 0)
                        {
                            //Console.WriteLine("Going down");
                            currentPosition += new Vector2(0, -1);
                        }
                        else if (yDist < 0)
                        {
                            //Console.WriteLine("Going up");
                            currentPosition += new Vector2(0, 1);
                        }

                        Hallway hall = new();

                        if (previous != null)
                        {
                            if (previous is Hallway h)
                            {
                                h.End = hall;
                            }

                            hall.Origin = previous;
                        }

                        PlaceHallAt(currentPosition, previous, hall);
                        previous = hall;
                    }
                }
            }

            connected.Add(nearest);
            connectedRooms++;
        }
    }


    // Generates rooms with a snowflake like
    // patern. Will start at the center of the
    // grid.
    public void Snowflake(int baseDist)
    {
        List<Vector2> directions = [new(baseDist, 0), new(-baseDist, 0), new(0, baseDist), new(0, -baseDist)];

        Snowflake(0, new Vector2(Width / 2, Height / 2), directions, baseDist);
        ConnectRoomsLinear();
        SetEndRoom();
    }

    // Recursive helper for Snowflake generation
    private void Snowflake(int recursions, Vector2 startPos, List<Vector2> directions, int baseDist)
    {
        if (!InBounds(startPos))
            return;

        if (!IsSpotEmpty(startPos))
            return;

        if (recursions > MaxRoomsPerBranch)
            return;

        if (recursions == 0)
        {
            StartPosition = startPos;
            PlaceAt(new Room(4, true), startPos);
        }
        else
        {
            PlaceAt(new Room(4, false), startPos);
        }

        rooms[(Room)GetAt(startPos)] = startPos;

        foreach (Vector2 direction in directions)
        {
            Vector2 nextStart = startPos + direction;

            if (recursions > 1)
            {
                int xOffset = 0;
                int yOffset = 0;

                if (Vector2.Normalize(direction) == Vector2.Unitx || Vector2.Normalize(direction) == -Vector2.Unitx)
                {
                    yOffset = random.Next(-1, 2);
                }

                if (Vector2.Normalize(direction) == Vector2.Unity || Vector2.Normalize(direction) == -Vector2.Unity)
                {
                    xOffset = random.Next(-1, 2);
                }

                nextStart.x += xOffset;
                nextStart.y += yOffset;
            }

            List<Vector2> dirs = [];

            if (Vector2.Normalize(direction) == Vector2.Unitx || Vector2.Normalize(direction) == -Vector2.Unitx)
            {
                dirs.Add(new Vector2(0, baseDist + recursions));
                dirs.Add(new Vector2(0, -baseDist - recursions));
            }

            if (Vector2.Normalize(direction) == Vector2.Unity || Vector2.Normalize(direction) == -Vector2.Unity)
            {
                dirs.Add(new Vector2(baseDist + recursions, 0));
                dirs.Add(new Vector2(-baseDist - recursions, 0));
            }

            Snowflake(recursions + 1, nextStart, dirs, baseDist);
        }
    }

    // Finds the nearest unconnected room
    public Room FindNearestUnconnected(Vector2 position, List<Room> connected)
    {
        Room nearest = rooms.Keys.First();
        float minDist = float.MaxValue;
        Connectable? current = GetAt(position);

        foreach (Room room in rooms.Keys)
        {
            if (current != null && room == current || connected.Contains(room))
            {
                continue;
            }

            if (Vector2.Distance(rooms[room], position) < minDist)
            {
                minDist = Vector2.Distance(rooms[room], position);
                nearest = room;
            }
        }

        return nearest;
    }*/

    // Places a hallway
    public bool PlaceHallAt(Vector2 pos, Connectable? origin, Connectable? end)
    {
        Hallway hall = Instantiate(hallPrefab.gameObject, 
            new Vector3(pos.x * 5, pos.y * 5, 0), Quaternion.identity).GetComponent<Hallway>();

        hall.Origin = origin;
        hall.End = end;
        
        if (PlaceAt(hall, pos))
        {
            halls.Add(hall, pos);
            return true;
        }

        Destroy(hall.gameObject);
        return false;
    }


    // Places an item at a grid position
    public bool PlaceAt(Connectable connectable, Vector2 pos)
    {
        if (IsSpotEmpty(pos))
        {
            grid[(int)pos.x, (int)pos.y] = connectable;

            if (connectable == null)
            {
                return false;
            }
            
            Instantiate(connectable.gameObject, new Vector3(pos.x * 5, pos.y * 5, 0), connectable.transform.rotation);
            return true;
        }
        
        Destroy(connectable.gameObject);
        return false;
    }

    // Gets the feature at a position
    public Connectable? GetAt(Vector2 pos)
    {
        return grid[(int)pos.x, (int)pos.y];
    }

    // Counts the rooms in the top half of the map
    public int CountTopHalf()
    {
        int bound = height / 2;
        int count = 0;

        foreach (Room room in rooms.Keys)
        {
            if (rooms[room].y < bound)
            {
                count++;
            }
        }

        return count;
    }

    // Counts the rooms in the bottom half of the map
    public int CountBottomHalf()
    {
        int bound = height / 2;
        int count = 0;

        foreach (Room room in rooms.Keys)
        {
            if (rooms[room].y > bound)
            {
                count++;
            }
        }

        return count;
    }

    // Counts the rooms in the left half of the map
    public int CountLeftHalf()
    {
        int bound = width / 2;
        int count = 0;

        foreach (Room room in rooms.Keys)
        {
            if (rooms[room].x < bound)
            {
                count++;
            }
        }

        return count;
    }

    // Counts the rooms in the right half of the map
    public int CountRightHalf()
    {
        int bound = width / 2;
        int count = 0;

        foreach (Room room in rooms.Keys)
        {
            if (rooms[room].x > bound)
            {
                count++;
            }
        }

        return count;
    }

    // Finds the least populated half of the grid
    public Half LeastPopulatedHalf()
    {
        int leftOrRight = Math.Min(CountRightHalf(), CountLeftHalf());
        int topOrBottom = Math.Min(CountBottomHalf(), CountTopHalf());

        int min = Math.Min(leftOrRight, topOrBottom);

        if (min == leftOrRight)
        {
            if (leftOrRight == CountLeftHalf())
            {
                return Half.Left;
            }

            return Half.Right;
        }

        if (topOrBottom == CountTopHalf())
        {
            return Half.Top;
        }

        return Half.Bottom;
    }

    // Calculates a penalty to slow increasing of the recursion depth
    private float CalculateDepthPenalty(int roomsGenerated)
    {
        float percentDepth = (float)roomsGenerated / maxRoomsPerBranch;
        float penaltyMultiplier = 1f - percentDepth;

        return penaltyMultiplier;
    }

    // x-coordinate distance between two rooms
    private int XDist(Room room1, Room room2)
    {
        Vector2 room1Pos = rooms[room1];
        Vector2 room2Pos = rooms[room2];

        return (int)(room1Pos.x - room2Pos.x);
    }

    // y-coordinate distance between two rooms.
    private int YDist(Room room1, Room room2)
    {
        Vector2 room1Pos = rooms[room1];
        Vector2 room2Pos = rooms[room2];

        return (int)(room1Pos.y - room2Pos.y);
    }

    // Determines if a position on the grid is empty.
    public bool IsSpotEmpty(Vector2 pos)
    {
        if (!InBounds(pos)) return false;

        return grid[(int)pos.x, (int)pos.y] is null;
    }

    // Determines if a position is within the grid
    private bool InBounds(Vector2 pos)
    {
        return pos.x >= 0 && pos.y >= 0 &&
               pos.x < width && pos.y < height;
    }

    // Determines if a room has only one neighbor
    public bool IsDeadEnd(Room room)
    {
        List<Vector2> directions = new();
        directions.Add(new Vector2(1, 0));
        directions.Add(new Vector2(-1, 0));
        directions.Add(new Vector2(0, -1));
        directions.Add(new Vector2(0, 1));
        
        List<Connectable> list = new();

        if (rooms.TryGetValue(room, out Vector2 pos))
        {
            foreach (Vector2 dir in directions)
            {
                Vector2 neighborPos = pos + dir;

                if (!InBounds(neighborPos))
                    continue;

                if (GetAt(neighborPos) is null)
                    continue;

                list.Add(GetAt(neighborPos));
            }
        }

        return list.Count == 1;
    }

    // Finds the farthest dead end room from the starting point
    public Room? GetFarthestDeadEnd()
    {
        Room farthest = (Room)GetAt(StartPosition);
        double farthestDistance = 0;

        foreach (Room room in rooms.Keys)
        {
            if (IsDeadEnd(room))
            {
                if (Vector2.Distance(StartPosition, rooms[room]) > farthestDistance)
                {
                    farthest = room;
                    farthestDistance = Vector2.Distance(StartPosition, rooms[room]);
                }
            }
        }

        return farthest;
    }

    // Sets the farthest dead end room as the end room
    public void SetEndRoom()
    {
        Room? farthest = GetFarthestDeadEnd();

        if (farthest is not null)
        {
            farthest.IsEndRoom = true;
            farthest.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    // Sets some rooms as "special" rooms
    public void SetSpecialRooms()
    {
        Queue<Room> roomQueue = new Queue<Room>();

        if (Start == null)
            return;

        roomQueue.Enqueue(Start);

        int generated = 0;

        while (roomQueue.Count > 0 && generated < specialRooms)
        {
            Room room = roomQueue.Dequeue();
            
            if ((float)random.NextDouble() < specialRoomsChance && !(room.IsSpecial || room.IsEndRoom || room.IsOrigin))
            {
                room.IsSpecial = true;
                room.GetComponent<Renderer>().material.color = Color.magenta;
                generated++;
            }

            if (room.Left != null)
                roomQueue.Enqueue((Room)room.Left);

            if (room.Right != null)
                roomQueue.Enqueue((Room)room.Right);

            if (room.Up != null)
                roomQueue.Enqueue((Room)room.Up);

            if (room.Down != null)
                roomQueue.Enqueue((Room)room.Down);
        }
    }
    
    /*
    // Prints a compressed view of the
    // grid.
    public void PrintCompressed()
    {
        for (int i = 0; i < Grid.GetLength(0); i++)
        {
            for (int j = 0; j < Grid.GetLength(1); j++)
            {
                if (Grid[i, j] is null)
                    Console.Write(" ");
                else
                    Console.Write(Grid[i, j]);
            }

            Console.WriteLine();
        }
    }

    // Breadth-first traversal of the tile graph
    public void BreadthFirstTraversal()
    {
        Queue<Room> roomQueue = new Queue<Room>();

        if (Start == null)
            return;

        roomQueue.Enqueue(Start);

        while (roomQueue.Count > 0)
        {
            Room room = roomQueue.Dequeue();
            Console.WriteLine(room);

            if (room.Left != null)
            {
                roomQueue.Enqueue((Room)room.Left);
                // Generate a hall between the current room
                // and the new room.
            }

            if (room.Right != null)
            {
                roomQueue.Enqueue((Room)room.Right);
                // Generate a hall between the current room
                // and the new room.
            }

            if (room.Up != null)
            {
                roomQueue.Enqueue((Room)room.Up);
                // Generate a hall between the current room
                // and the new room.
            }

            if (room.Down != null)
            {
                roomQueue.Enqueue((Room)room.Down);
                // Generate a hall between the current room
                // and the new room.
            }
        }
    }
    */

    public override string ToString()
    {
        string s = string.Empty;

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] is null)
                {
                    s += "- ";
                    continue;
                }

                s += grid[i, j] + " ";
            }

            s += '\n';
        }

        return s;
    }
}
