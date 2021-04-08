using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;

    public float distance = 13.0f;
    public float height = 8.0f;
    public float smoothTime = 0.3f;
    Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = target.position + new Vector3(0, height, 0) + target.forward * -distance;
        transform.LookAt(target);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {

    }

    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position + new Vector3(0, height, 0) + target.forward * -distance, ref velocity, smoothTime);
        transform.LookAt(target);
    }
}
