using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPopUp : MonoBehaviour
{
    
    public GameObject settingWindow;

    public void OnClickSettingButton()
    {
        settingWindow.gameObject.SetActive(true);
    }
    
    
}
