using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeFix : MonoBehaviour
{
    private void Awake()
    {
        Screen.SetResolution(1980, 1080, true, 144);
    }
}
