using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCollisionitem : MonoBehaviour
{

    public GameObject car;
    public float DestroyTime = 3.0f;

    
    void Start()
    {
        Destroy(this.gameObject, DestroyTime);
    }

    void Update()
    {
        transform.position = new Vector3(car.transform.position.x, car.transform.position.y, car.transform.position.z);
    }
    
}

