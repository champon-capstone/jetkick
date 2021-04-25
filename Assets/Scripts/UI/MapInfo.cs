using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapInfo
{
    private string mapName;
    private string mapDescription;
    private Sprite mapImage;
    
    public string MapName
    {
        get => mapName;
        set => mapName = value;
    }

    public string MapDescription
    {
        get => mapDescription;
        set => mapDescription = value;
    }

    public Sprite MapImage
    {
        get => mapImage;
        set => mapImage = value;
    }
}
