using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfoGenerator
{
    private ArrayList mapList;

    public MapInfoGenerator()
    {
        mapList = new ArrayList();

        mapList.Add(new MapInfo("ObstacleMap", "다른 맵과는 다르게 여러 장애물이 있고 아이템이 존재하는 맵입니다.", Resources.Load<Sprite>("obstacle")));
        mapList.Add(new MapInfo("CityMap", "도심 한가운데에 맵 종류 중 가장 크기가 작은 옥상이 제트킥의 무대가 됩니다. ", Resources.Load<Sprite>("citymap")));
        mapList.Add(new MapInfo("IslandMap", "지구 어딘가에 존재하는 신비한 섬입니다. 울퉁불퉁한 지형이 조작이 어려울 것입니다.", Resources.Load<Sprite>("islandmap")));
    }
    
    public ArrayList MapList
    {
        get => mapList;
    }
}
