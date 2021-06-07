using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCountdown : MonoBehaviour
{
    public Text TimeCount;
    public float TimeCost;
    private float minute;
    private float second;

    // Update is called once per frame
    void Update()
    {
        if(TimeCost > 0)
        {
            TimeCost -= Time.deltaTime;
            minute = TimeCost / 60.0f;
            second = TimeCost % 60.0f;
            TimeCount.text = "남은시간 :" + string.Format("{0:f0}"+ ":", minute) + string.Format("{0:f0}",second);
        }
        else if(TimeCost <= 0)
        {
            //Time over situation
            Debug.Log("타임끝!");
        }
        
    }
}
