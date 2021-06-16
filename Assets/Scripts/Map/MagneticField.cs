using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticField : MonoBehaviour
{
    public float speed = 100.0f;

    float delta = 0.01f;
    float interval = 0.01f;
    float time = 0.0f;
    public TimeCountdown timecountdown;
    public MultiCar[] TargetCar; //Test Destroy target car 

    // Start is called before the first frame update
    void Start()
    {
        interval = delta / speed;
        timecountdown = GameManager.instance.GetComponent<TimeCountdown>();
        //TargetCar = GameObject.Find("PlayerCamera").GetComponent<PlayerCamera>().target.gameObject;
        //TargetCar = FindObjectOfsType<MultiCar>().gameObject;
        TargetCar = FindObjectsOfType<MultiCar>();
    }

    // Update is called once per frame
    void Update()
    {
        if (time > interval && timecountdown.EndTime())
        {
            //Debug.Log("자기장 축소들어왔냐??");
            
            transform.localScale -= new Vector3(delta, delta, delta);
            time = 0.0f;
            if(transform.localScale.x <= 0.0f && transform.localScale.y <= 0.0f && transform.localScale.z <= 0.0f)
            {
                Destroy(gameObject);
            }
            // (ring out)player destroy everyone 
            for(int i=0; i<TargetCar.Length;i++)
            {
                if (TargetCar[i] != null && 
                    Vector3.Distance(gameObject.transform.position, TargetCar[i].transform.position) > transform.localScale.x * 0.5)
                {
                    Debug.Log("자동차 폭발!!!!!");
                    GameObject BigExplosion;
                    BigExplosion = Resources.Load("BigExplosion") as GameObject;
                    Instantiate(BigExplosion, TargetCar[i].transform.position, Quaternion.identity);
                    Destroy(TargetCar[i].gameObject);
                }
            }
            
        }
        time += Time.deltaTime;
    }
}
