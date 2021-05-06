using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour
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
        transform.Translate(Vector3.left * speed * Time.deltaTime * 5f);
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
           
            rb = collision.GetComponentInParent<Rigidbody>();
            
            GameObject BigExplosion;
            BigExplosion = Resources.Load("BigExplosion") as GameObject;
            Instantiate(BigExplosion, collision.gameObject.transform.position, Quaternion.identity);
            collision.attachedRigidbody.AddForce(Vector3.left * power * 100000.0f);
            Destroy(gameObject);
        }
    }
}
