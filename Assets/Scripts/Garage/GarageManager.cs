using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GarageManager : MonoBehaviour
{
    public Text CarName;
    public Text CarDetail;
    public GameObject CarSpawnPoint;
    public GameObject Parent;
    public GameObject[] Cars = new GameObject[5];

    private bool Startchoose = true;
    private string[] Names = {"����1����","����2����","����3����" ,"����1����","����5����" };
    private string[] Details = { "������ 1�����ν� �ƹ�ư 1����",
                                 "������ 2�����ν� �ƹ�ư 2����",
                                 "������ 3�����ν� �ƹ�ư 3����",
                                 "������ 4�����ν� �ƹ�ư 4����",
                                 "������ 5�����ν� �ƹ�ư 5����" };
    
    GameObject currentCar;
    
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
        if(!Startchoose)
        {
            for(int f=1; f<6; f++)
            {
                if(GameObject.Find("Car"+f+"(Clone)") !=null)
                {
                    Destroy(GameObject.Find("Car" + f + "(Clone)"));
                }
            }
        }
        else
        {
            Startchoose = false;
        }
        /*Debug.Log("CallCar �Ҹ�");
        Debug.Log("�Ѿ�� ��"+ i);
        Debug.Log(Names[i]);
        Debug.Log(Details[i]);*/

        //choose car number is i
        CarName.text = Names[i];
        CarDetail.text = Details[i];

        
        if(i == 1)
        {
            CarSpawnPoint.transform.position = new Vector3(CarSpawnPoint.transform.position.x, CarSpawnPoint.transform.position.y - 0.4f, CarSpawnPoint.transform.position.z);
            currentCar = Instantiate(Cars[i], CarSpawnPoint.transform.position, Quaternion.identity, Parent.transform);
            CarSpawnPoint.transform.position = new Vector3(CarSpawnPoint.transform.position.x, CarSpawnPoint.transform.position.y + 0.4f, CarSpawnPoint.transform.position.z);
        }
        else if(i == 2)
        {
            CarSpawnPoint.transform.position = new Vector3(CarSpawnPoint.transform.position.x, CarSpawnPoint.transform.position.y - 0.1f, CarSpawnPoint.transform.position.z);
            currentCar = Instantiate(Cars[i], CarSpawnPoint.transform.position, Quaternion.identity, Parent.transform);
            CarSpawnPoint.transform.position = new Vector3(CarSpawnPoint.transform.position.x, CarSpawnPoint.transform.position.y + 0.1f, CarSpawnPoint.transform.position.z);
        }
        
        else
        {
            currentCar = Instantiate(Cars[i], CarSpawnPoint.transform.position, Quaternion.identity, Parent.transform);
        }
        currentCar.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        currentCar.transform.localEulerAngles = new Vector3(0, 180.0f, 0);
        //Instantiate part
        //Instantiate(BigExplosion, TargetCar[i].transform.position, Quaternion.identity);
    }

    public void CallOk()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {{"car", currentCar.name}});
    }
    
}
