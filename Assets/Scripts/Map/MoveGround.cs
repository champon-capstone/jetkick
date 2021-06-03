using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGround : MonoBehaviour
{
    public GameObject go;
    public GameObject back;
    public GameObject target;
    public float Speed;
    // Start is called before the first frame update
    void Start()
    {
        target = back;
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, target.transform.position, Time.deltaTime * Speed);
        if (Vector3.Distance(gameObject.transform.position, target.transform.position) < 1.0f)
        {
            if (target == back)
            {
                target = go;
            }
            else
            {
                target = back;
            }
        }

    }
}
