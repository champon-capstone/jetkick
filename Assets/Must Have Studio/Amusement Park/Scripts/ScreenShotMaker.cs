using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotMaker : MonoBehaviour
{
    public string Name = "Sample name";
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
if(Input.anyKeyDown)
                ScreenCapture.CaptureScreenshot(Name+".png");
    }
}
