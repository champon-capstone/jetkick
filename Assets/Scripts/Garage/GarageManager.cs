using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public class GarageManager : MonoBehaviour
{
    public Text CarName;
    public Text CarDetail;
    public GameObject CarSpawnPoint;
    public GameObject Parent;
    public GameObject[] Cars = new GameObject[5];
    public GameObject currentCar;

    private bool Startchoose = true;
    private string[] Names = { "JB 700W", "라피드 GT 클래식", "알파", "GT500", "퓨지티브" };
    private string[] Details = { "차에 탑승해서, 달릴 준비를 하세요. 몸매가 제대로 빠진 이 녀석의 출격을 기다리고 있습니다. 클래식한 분위기를 뽐내는 JB 700W를 타고 밖으로 나가세요.",
                                 "구시대에 나온 것들은 죄다 낡았습니다만 라피드 GT 클래식은 성숙하게 발전했습니다. 물론 새로운 스포츠 카에 비할 바는 못되겠지만 오직 시간만이 말해줄 수 있는 자신감이 무엇인지 알게 될 겁니다.",
                                 "알파는 현대적 성능과 멋스러운 디자인을 고급 자동차만의 클래식한 멋과 융합한 자동차이며 말끔하고, 빠릅니다. 자신이 운전중이라는 것을 깜빡할 정도로요.",
                                 "스마트 안전장치나 하이테크 주행 지원은 없습니다. 하지만 이 차가 당신에게 제공하는 것도 있습니다. 바로 이 차 덕분에 당신이 얼마나 멋있어 보이는지 생각해보세요.",
                                 "엔지니어링과 설계방식을 매우 신중하게 적용했습니다. 이 퓨지티브 모델은 마음에 들지 않는 상사의 남은 머리카락 한 가닥까지 날려버릴 수 있습니다." };

    // Start is called before the first frame update
    void Start()
    {
        CarName.text = Names[0];
        CarDetail.text = Details[0];
        currentCar = Instantiate(Cars[0], CarSpawnPoint.transform.position, Quaternion.identity, Parent.transform);
        currentCar.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        currentCar.transform.localEulerAngles = new Vector3(0, 180.0f, 0);
        Startchoose = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CallCar(int i)
    {
        if (!Startchoose)
        {
            for (int f = 1; f < 6; f++)
            {
                if (GameObject.Find("Car" + f + "(Clone)") != null)
                {
                    Destroy(GameObject.Find("Car" + f + "(Clone)"));
                }
            }
        }
        else
        {
            Startchoose = false;
        }
        /*Debug.Log("CallCar 불림");
        Debug.Log("넘어온 값"+ i);
        Debug.Log(Names[i]);
        Debug.Log(Details[i]);*/

        //choose car number is i
        CarName.text = Names[i];
        CarDetail.text = Details[i];


        if (i == 1)
        {
            CarSpawnPoint.transform.position = new Vector3(CarSpawnPoint.transform.position.x, CarSpawnPoint.transform.position.y - 0.4f, CarSpawnPoint.transform.position.z);
            currentCar = Instantiate(Cars[i], CarSpawnPoint.transform.position, Quaternion.identity, Parent.transform);
            CarSpawnPoint.transform.position = new Vector3(CarSpawnPoint.transform.position.x, CarSpawnPoint.transform.position.y + 0.4f, CarSpawnPoint.transform.position.z);
        }
        else if (i == 2)
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
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "car", currentCar.name } });
    }

    public void returnLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

}
