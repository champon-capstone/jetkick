using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyController : MonoBehaviour
{
    public float DestroyTime=1.0f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, DestroyTime);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
