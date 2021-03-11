using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Used by CarCamera to setup up different styled camera views.
[System.Serializable]
public class CarCameraSettings
{
    // How far the camera is behind the car.
    public float distance = 6.0f;

    // How far the camera is above the car.
    public float height = 3.0f;

    // Smoothing transition time of the Camera.
    public float smoothTime = 0.3F;


    // A default settings of a camera.
    /// <returns></returns>
    public static CarCameraSettings GetDefaultSettings0() {
        CarCameraSettings carCameraSettings = new CarCameraSettings();
        carCameraSettings.distance = 6.0f;
        carCameraSettings.height = 3.0f;
        carCameraSettings.smoothTime = 0.3f;

        return carCameraSettings;
    }

    /// A default settings of a camera.
    /// <returns></returns>
    public static CarCameraSettings GetDefaultSettings1()
    {
        CarCameraSettings carCameraSettings = new CarCameraSettings();
        carCameraSettings.distance = 6.0f;
        carCameraSettings.height = 3.0f;
        carCameraSettings.smoothTime = 0.3f;

        return carCameraSettings;
    }

    /// A default settings of a camera.
    /// <returns></returns>
    public static CarCameraSettings GetDefaultSettings2()
    {
        CarCameraSettings carCameraSettings = new CarCameraSettings();
        carCameraSettings.distance = 6.0f;
        carCameraSettings.height = 3.0f;
        carCameraSettings.smoothTime = 0.3f;

        return carCameraSettings;
    }
}
