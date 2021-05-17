using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticField : MonoBehaviour
{
    public float speed = 100.0f;

    float delta = 0.01f;
    float interval = 0.01f;
    float time = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        interval = delta / speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (time > interval)
        {
            transform.localScale -= new Vector3(delta, delta, delta);
            time = 0.0f;
        }
        time += Time.deltaTime;
    }
}
