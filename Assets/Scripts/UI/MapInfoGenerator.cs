using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfoGenerator
{
    private ArrayList mapList;

    public MapInfoGenerator()
    {
        mapList = new ArrayList();

        mapList.Add(new MapInfo("ObstacleMap", "다른맵들과는 다르게 여러 장애물이 있는 위험한 무대에서 밀치고 떨어뜨리는 아이템전의 무대! 당신은 살아남을 수 있을까요?", Resources.Load<Sprite>("obstacle")));
        mapList.Add(new MapInfo("CityMap", "도심 한가운데에 가장 크기가 작은 옥상이 제트킥의 무대가 됩니다. 무대가 작은만큼 더욱 심리적인 싸움을 해야 할 것입니다.", Resources.Load<Sprite>("citymap")));
        mapList.Add(new MapInfo("IslandMap", "지구 어딘가에 존재하는 신비한 섬,  울퉁불퉁한 지형으로 서로를 밀어내기 위해선 플레이어에게 섬세한 컨트롤이 요구됩니다.", Resources.Load<Sprite>("islandmap")));
    }
    
    public ArrayList MapList
    {
        get => mapList;
    }
}
