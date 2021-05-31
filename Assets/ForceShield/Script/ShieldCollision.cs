using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCollision : MonoBehaviour
{

    public GameObject car;

    void Start()
    {
  
 

    }

    void Update()
    {
        transform.position =new Vector3(car.transform.position.x,car.transform.position.y,car.transform.position.z);
        
       

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Missile")
        {
            Debug.Log("실드 미사일 막음");
            Destroy(other.transform.parent.gameObject);
        }
        if (other.gameObject.tag == "Banana")
        { 

            Debug.Log("실드 바나나 막음");
            Destroy(other.gameObject);
        }

       
    }

    
}

