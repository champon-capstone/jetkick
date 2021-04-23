using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float distance;
    public bool boost = true;
    public float speed = 3f;
    public float offset = 0f;

    private bool isForward = true;
    private Vector3 startPos;
    // Start is called before the first frame update
    void Awake()
    {
        startPos = transform.position;
        transform.position += Vector3.up * offset;
    }

    // Update is called once per frame
    void Update()
    {
        if(boost)
        {
            if (isForward)
            {
                if (transform.position.y < startPos.y + distance)
                {
                    transform.position += Vector3.up * Time.deltaTime * speed;
                }
                else
                {
                    isForward = false;
                    boost = false;
                    Invoke("WaitForIt", 3);
                }
            }
            else
            {
                if (transform.position.y > startPos.y)
                {
                    transform.position -= Vector3.up * Time.deltaTime * speed;
                }
                else
                {
                    isForward = true;
                    boost = false;
                    Invoke("WaitForIt", 3);
                }
            }
        }
    }

    void WaitForIt()
    {
        //Debug.Log("5√ ¡ˆ≥≤");
        boost = true;
    }
}
