using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.RainMaker;

public class WeatherManager : MonoBehaviour
{
    public Weather weather;
    public WeatherSettings[] weatherSettings = new WeatherSettings[]
    {
    };

    public BaseRainScript rainPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ChangeSkyBox(weatherSettings[(int)weather].skybox);
        ChangeRain(weatherSettings[(int)weather].rainIntensity);
    }

    void ChangeSkyBox(Material skybox)
    {
        RenderSettings.skybox = skybox;
    }

    void ChangeRain(float rainIntensity)
    {
        if (rainIntensity > 1.0f)
        {
            rainIntensity = 1.0f;
        }
        else if (rainIntensity < 0)
        {
            rainIntensity = 0;
        }
        rainPrefab.gameObject.SetActive(rainIntensity > 0);
        rainPrefab.RainIntensity = rainIntensity;
    }
}
