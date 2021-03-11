using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Camera script to follow car while driving
public class CarCamera : MonoBehaviour
{ 

    // target of the camera to follow
    public Transform target;

    /// SmoothDamp velocity
    private Vector3 velocity = Vector3.zero;

    /// all camera settings
    public List<CarCameraSettings> carCameraSettingsList;

  
    /// current index of camera settings used
    public int cameraSettingsIndex = 0;

    private void Start()
    {
        ///set the start position to the camera
        Vector3 newPosition = target.position + new Vector3(0, carCameraSettingsList[cameraSettingsIndex].height, 0) + target.forward * -carCameraSettingsList[cameraSettingsIndex].distance;
        this.transform.position = newPosition;
        this.transform.LookAt(target);
    }

    void LateUpdate()
    {

        //return if there is no camera settings
        if (carCameraSettingsList == null)
            return;
        if (carCameraSettingsList.Count == 0)
            return;


        //switch camera settings to F1 button press
        if (Input.GetKeyDown(KeyCode.F1))
        {
            cameraSettingsIndex++;

            if (cameraSettingsIndex > carCameraSettingsList.Count - 1)
                cameraSettingsIndex = 0;
        }


        //calculate new position of the camera
        Vector3 newPosition = target.position + new Vector3(0, carCameraSettingsList[cameraSettingsIndex].height, 0) + target.forward * -carCameraSettingsList[cameraSettingsIndex].distance;

        //transition smoothly to new position from old position
        this.transform.position = Vector3.SmoothDamp(this.transform.position, newPosition, ref velocity, carCameraSettingsList[cameraSettingsIndex].smoothTime);
      
        //look towards the target
        this.transform.LookAt(target);
    }
}