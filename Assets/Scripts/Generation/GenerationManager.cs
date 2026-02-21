using System.Collections.Generic;
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
        GenerateFloorLayout();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Y))
        {
            GenerateFloorLayout();
        }
#endif
    }

    public void GenerateFloorLayout()
    {
        List<Connectable> roomCollection = new List<Connectable>(FindObjectsByType<Connectable>(FindObjectsSortMode.None));
        foreach (Connectable room in roomCollection)
        {
            Destroy(room.gameObject);
        }

        TileGraph map = new TileGraph(mapWidth, mapHeight, maxRoomsPerBranch);

        map.roomPrefab = roomPrefab;
        map.hallPrefab = hallPrefab;

        map.GenerateMap(new(map.Width / 2, map.Height / 2));
    }
}