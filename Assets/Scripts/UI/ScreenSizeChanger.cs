using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSizeChanger : MonoBehaviour
{
    public Dropdown sizeDropdown;
    
    private Dictionary<string, int[]> sizeMap;

    private void Start()
    {
        sizeMap = new Dictionary<string, int[]>();
        sizeMap.Add("1280X720", new[] {1280, 720});
        sizeMap.Add("1920X1080", new[] {1920, 1080});
        sizeMap.Add("640X480", new[] {640, 480});
    }

    public void OnSizeChange()
    {
        var selectedValue = sizeDropdown.options[sizeDropdown.value].text;
        if (sizeMap.ContainsKey(selectedValue))
        {
            var size = sizeMap[selectedValue];
            Screen.SetResolution(size[0], size[1], false, 144);
        }
    }
}


