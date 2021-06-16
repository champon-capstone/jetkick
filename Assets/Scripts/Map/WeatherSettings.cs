using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSettings : MonoBehaviour
{
    public Material skybox;

    [Range(0.0f, 1.0f)]
    public float rainIntensity = 0;

    [Range(0.0f, 1.0f)]
    public float lightIntensity = 1.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
