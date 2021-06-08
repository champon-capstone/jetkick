using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCountdown : MonoBehaviour
{
    public Text TimeCount;
    public Text Explanation;
    public float TimeCost;
    public float PlusTimeCost;
    public GameObject MagneticField;
    public GameObject SpawnMagnetic;


    private int minute;
    private int second;
    private bool endgame = false;
    private bool endtime;
    // Update is called once per frame
    void Update()
    {
        if(TimeCost > 0)
        {
            TimeCost -= Time.deltaTime;
            minute = (int)(TimeCost / 60);
            second = (int)(TimeCost % 60.0f);
            if(endgame)
            {
                //TimeCount.text = "�ڱ��� ��� �����ð�: " + string.Format("{0:f0}" + ":", minute) + string.Format("{0:f0}", second);
                Explanation.text = "���� �������� �̵��ϼ���";
            }
            else
            {
                //TimeCount.text = "���ѱ��� ���� �����ð�: " + string.Format("{0:f0}" + ":", minute) + string.Format("{0:f0}", second);
                Explanation.text = "������ ��� ���� �¸��ϼ���";
            }
            TimeCount.text = "�����ð�: " + string.Format("{0:f0}" + ":", minute) + string.Format("{0:f0}", second);

        }
        else if(TimeCost <= 0)
        {
            //Time over situation
            if(endgame)// Starting smaller magneticField 
            {
                Debug.Log("�ڱ��� ��ҽ���");
                Explanation.text = "���ѱ����� ��ҵ˴ϴ�.\n ������ ��� ���� �¸��ϼ���";
                endtime = true;
            }
            else// Create magneticField
            {
                Debug.Log("�ڱ��� ����");
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
