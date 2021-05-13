using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSizeChanger : MonoBehaviour
{
    public GameObject settingWindow;
    public Dropdown sizeDropdown;

    private int fps = 144;
    private Dictionary<string, int[]> sizeMap;
    private bool isFullScreen = false;

    private void Start()
    {
        Screen.SetResolution(1280, 720, false, fps);
        sizeMap = new Dictionary<string, int[]>();
        sizeMap.Add("1280X720", new[] {1280, 720});
        sizeMap.Add("1920X1080", new[] {1920, 1080});
        sizeMap.Add("640X480", new[] {640, 480});
    }

    public void ChangeSize()
    {
        var selectedValue = sizeDropdown.options[sizeDropdown.value].text;
        if (sizeMap.ContainsKey(selectedValue))
        {
            var size = sizeMap[selectedValue];
            Screen.SetResolution(size[0], size[1], isFullScreen, fps);
        }
    }

    public void OnFullScreenToggled()
    {
        isFullScreen = !isFullScreen;
    }
    
    public void OnSixToggled()
    {
        fps = 60;
    }

    public void OnNineToggled()
    {
        fps = 90;
    }
    
    public void OnOneFourFourToggled()
    {
        fps = 144;
    }
}


