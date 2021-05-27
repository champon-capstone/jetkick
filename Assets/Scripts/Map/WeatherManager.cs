using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public Weather weather;
    public WeatherSettings[] weatherSettings = new WeatherSettings[]
    {
    };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ChangeSkyBox(weatherSettings[(int)weather].skybox);
    }

    void ChangeSkyBox(Material skybox)
    {
        RenderSettings.skybox = skybox;
    }
}
