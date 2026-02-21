using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    public Connectable roomPrefab;
    public Connectable hallPrefab;

    void Start()
    {
        TileGraph map = new TileGraph(11, 11,7);
        
        map.roomPrefab = roomPrefab;
        map.hallPrefab = hallPrefab;
        
        map.GenerateMap(new(map.Width / 2, map.Height / 2));
    }
}