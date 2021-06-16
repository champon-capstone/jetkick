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
    public Light light;

    // Start is called before the first frame update
    void Start()
    {
        if (rainPrefab == null)
        {
            rainPrefab = FindObjectOfType(typeof(BaseRainScript)) as BaseRainScript;
        }
        if (light == null)
        {
            light = FindObjectOfType(typeof(Light)) as Light;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ChangeSkyBox(weatherSettings[(int)weather].skybox);
        ChangeRain(weatherSettings[(int)weather].rainIntensity);
        ChangeLight(weatherSettings[(int)weather].lightIntensity);
    }

    void ChangeSkyBox(Material skybox)
    {
        RenderSettings.skybox = skybox;
    }

    void ChangeRain(float rainIntensity)
    {
        rainPrefab.gameObject.SetActive(rainIntensity > 0);
        rainPrefab.RainIntensity = rainIntensity;
    }

    void ChangeLight(float lightIntensity)
    {
        light.intensity = lightIntensity;
    }
}
