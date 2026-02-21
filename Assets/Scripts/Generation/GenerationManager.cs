using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    public Connectable roomPrefab;
    public Connectable hallPrefab;

    public int mapWidth;
    public int mapHeight;
    public int maxRoomsPerBranch;
    
    void Start()
    {
        TileGraph map = new TileGraph(mapWidth, mapHeight,maxRoomsPerBranch);
        
        map.roomPrefab = roomPrefab;
        map.hallPrefab = hallPrefab;
        
        map.GenerateMap(new(map.Width / 2, map.Height / 2));
    }
}