using UnityEngine;

public enum RoomType
{
    StartRoom,
    EndRoom,
    SpecialRoom,
    Room,
    Hallway,
}

[CreateAssetMenu(fileName = "RoomData", menuName = "ScriptableObjects/RoomData", order = 1)]
public class RoomDataScript : ScriptableObject
{
    [Header("Room Appearance")]
    public float width;
    public float height;
    public Sprite sprite;
    
    [Header("Room Type")]
    public RoomType roomType;
    
    [Header("Door Positions")]
    public Vector3 upDoorPosition;
    public Vector3 rightDoorPosition;
    public Vector3 leftDoorPosition;
    public Vector3 downDoorPosition;
}
