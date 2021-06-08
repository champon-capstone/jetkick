using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GarageManager : MonoBehaviour
{
    public Text CarName;
    public Text CarDetail;
    public GameObject CarSpawnPoint;

    private string[] Names = {"차량1번임","차량2번임","차량3번임" ,"차량1번임","차량5번임" };
    private string[] Details = { "이차는 1번으로써 아무튼 1번임",
                                 "이차는 2번으로써 아무튼 2번임",
                                 "이차는 3번으로써 아무튼 3번임",
                                 "이차는 4번으로써 아무튼 4번임",
                                 "이차는 5번으로써 아무튼 5번임" };
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
        /*Debug.Log("CallCar 불림");
        Debug.Log("넘어온 값"+ i);
        Debug.Log(Names[i]);
        Debug.Log(Details[i]);*/
        //choose car number is i
        CarName.text = Names[i];
        CarDetail.text = Details[i];

        //Instantiate part
        //Instantiate(BigExplosion, TargetCar[i].transform.position, Quaternion.identity);
    }

    
}
