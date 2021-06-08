using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfoGenerator
{
    private ArrayList mapList;

    public MapInfoGenerator()
    {
        mapList = new ArrayList();

        mapList.Add(new MapInfo("ObstacleMap", "ObstableMap Description", Resources.Load<Sprite>("obstacle")));
        mapList.Add(new MapInfo("CityMap", "CityMap Description", Resources.Load<Sprite>("citymap")));
        mapList.Add(new MapInfo("IslandMap", "islandMap Description", Resources.Load<Sprite>("islandmap")));
    }
    
    public ArrayList MapList
    {
        get => mapList;
    }
}
