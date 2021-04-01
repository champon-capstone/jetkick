using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used by CarCamera to setup up different styled camera views.
/// </summary>
[System.Serializable]
public class CarCameraSettings
{
    /// <summary>
    /// How far the camera is behind the car.
    /// </summary>
    public float distance = 13.0f;

    /// <summary>
    /// How far the camera is above the car.
    /// </summary>
    public float height = 8.0f;

    /// <summary>
    /// Smoothing transition time of the Camera.
    /// </summary>
    public float smoothTime = 0.3f;

    public CarCameraSettings()
    {
    }

    public CarCameraSettings(float distance, float height, float smoothTime)
    {
        this.distance = distance;
        this.height = height;
        this.smoothTime = smoothTime;
    }

    /// <summary>
    /// A default settings of a camera.
    /// </summary>
    /// <returns></returns>
    public static CarCameraSettings GetDefaultSettings0() => new CarCameraSettings();

    /// <summary>
    /// A default settings of a camera.
    /// </summary>
    /// <returns></returns>
    public static CarCameraSettings GetDefaultSettings1() => new CarCameraSettings(20.0f, 15.0f, 0.3f);

    /// <summary>
    /// A default settings of a camera.
    /// </summary>
    /// <returns></returns>
    public static CarCameraSettings GetDefaultSettings2() => new CarCameraSettings(6.0f, 3.0f, 0.3f);
}
