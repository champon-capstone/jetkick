using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCountdown : MonoBehaviour
{
    public Text TimeCount;
    public float TimeCost;
    public float PlusTimeCost;
    public GameObject MagneticField;
    public GameObject SpawnMagnetic;


    private float minute;
    private float second;
    private bool endgame = false;
    private bool endtime = false;
    // Update is called once per frame
    void Update()
    {
        if(TimeCost > 0)
        {
            TimeCost -= Time.deltaTime;
            minute = TimeCost / 60.0f;
            second = TimeCost % 60.0f;
            if(endgame)
            {
                TimeCount.text = "자기장 축소 남은시간: " + string.Format("{0:f0}" + ":", minute) + string.Format("{0:f0}", second);
            }
            else
            {
                TimeCount.text = "제한구역 생성 남은시간: " + string.Format("{0:f0}" + ":", minute) + string.Format("{0:f0}", second);
            }
            
        }
        else if(TimeCost <= 0)
        {
            //Time over situation
            if(endgame)// Starting smaller magneticField 
            {
                Debug.Log("자기장 축소시작");
                endtime = true;
            }
            else// Create magneticField
            {
                Debug.Log("자기장 생성");
                Instantiate(MagneticField, SpawnMagnetic.transform.position, Quaternion.identity);
                TimeCost = PlusTimeCost;//plus time
                endgame = true;
            }
            

        }
    }


    public bool EndTime()
    {
        return endtime;
    }
}
