using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileMove : MonoBehaviour
{
    public float DestroyTime = 4.0f;
    public float MissileSpeed=32.0f;
    // Start is called before the first frame update
    void Start()
    {
        // Destroy(this.gameObject.transform.parent.gameObject, DestroyTime);
        Destroy(gameObject, DestroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * MissileSpeed * Time.deltaTime);
        
    }
}
