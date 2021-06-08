using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GarageManager : MonoBehaviour
{
    public Text CarName;
    public Text CarDetail;
    public GameObject CarSpawnPoint;

    private string[] Names = {"����1����","����2����","����3����" ,"����1����","����5����" };
    private string[] Details = { "������ 1�����ν� �ƹ�ư 1����",
                                 "������ 2�����ν� �ƹ�ư 2����",
                                 "������ 3�����ν� �ƹ�ư 3����",
                                 "������ 4�����ν� �ƹ�ư 4����",
                                 "������ 5�����ν� �ƹ�ư 5����" };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CallCar(int i)
    {
        /*Debug.Log("CallCar �Ҹ�");
        Debug.Log("�Ѿ�� ��"+ i);
        Debug.Log(Names[i]);
        Debug.Log(Details[i]);*/
        //choose car number is i
        CarName.text = Names[i];
        CarDetail.text = Details[i];

        //Instantiate part
        //Instantiate(BigExplosion, TargetCar[i].transform.position, Quaternion.identity);
    }

    
}
