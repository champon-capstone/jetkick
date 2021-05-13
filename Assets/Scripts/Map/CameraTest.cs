using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour
{
    public Camera Carcamera;
    public Camera Mapcamera;

    public static bool die = false;
    // Start is called before the first frame update
    void Start()
    {
        Carcamera.enabled = true;
        Mapcamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Cardead()
    {
        Debug.Log("cardead call");
        Carcamera.enabled = false;
        Mapcamera.enabled = true;
    }
}
