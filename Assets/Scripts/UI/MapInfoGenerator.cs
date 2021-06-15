using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfoGenerator
{
    private ArrayList mapList;

    public MapInfoGenerator()
    {
        mapList = new ArrayList();

        mapList.Add(new MapInfo("ObstacleMap", "�ٸ��ʵ���� �ٸ��� ���� ��ֹ��� �ִ� ������ ���뿡�� ��ġ�� ����߸��� ���������� ����! ����� ��Ƴ��� �� �������?", Resources.Load<Sprite>("obstacle")));
        mapList.Add(new MapInfo("CityMap", "���� �Ѱ���� ���� ũ�Ⱑ ���� ������ ��Ʈű�� ���밡 �˴ϴ�. ���밡 ������ŭ ���� �ɸ����� �ο��� �ؾ� �� ���Դϴ�.", Resources.Load<Sprite>("citymap")));
        mapList.Add(new MapInfo("IslandMap", "���� ��򰡿� �����ϴ� �ź��� ��,  ���������� �������� ���θ� �о�� ���ؼ� �÷��̾�� ������ ��Ʈ���� �䱸�˴ϴ�.", Resources.Load<Sprite>("islandmap")));
    }
    
    public ArrayList MapList
    {
        get => mapList;
    }
}
