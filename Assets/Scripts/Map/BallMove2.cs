using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove2 : MonoBehaviour
{
    public float speed = 80000f;
    public float destroyTime = 2.0f;
    public float power = 10f;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        //rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime * 5f);
        Destroy(gameObject, destroyTime);
    }
    
}
