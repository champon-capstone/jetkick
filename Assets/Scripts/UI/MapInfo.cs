using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapInfo
{
    private string mapName;
    private string mapDescription;
    private Image mapImage;
    
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

    public Image MapImage
    {
        get => mapImage;
        set => mapImage = value;
    }
}
