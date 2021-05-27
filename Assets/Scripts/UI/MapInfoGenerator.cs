using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfoGenerator
{
    private ArrayList mapList;

    public MapInfoGenerator()
    {
        mapList = new ArrayList();

        mapList.Add(new MapInfo("TestMap", "testMap Description", Resources.Load<Sprite>("test")));
        mapList.Add(new MapInfo("CityMap", "CityMap Description", Resources.Load<Sprite>("test")));
        mapList.Add(new MapInfo("IslandMap", "islandMap Description", Resources.Load<Sprite>("test")));
    }
    
    public ArrayList MapList
    {
        get => mapList;
    }
}
