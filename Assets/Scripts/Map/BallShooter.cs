using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BallShooter : MonoBehaviour
{
   
    public GameObject Yball;
    public GameObject Rball;
    public GameObject Bball;
    public float interval = 3.0f;
    public float rnd;
    public float offset;
    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
       
        InvokeRepeating("Shoot", 0.5f+offset, interval);
    }

    void Shoot()
    {
        rnd = Random.Range(0, 3);
        pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        if (rnd == 0)
        {
            PhotonNetwork.Instantiate("Rball", transform.position, transform.rotation);
        }
        else if(rnd == 1)
        {
            PhotonNetwork.Instantiate("Yball", transform.position, transform.rotation);
        }
        else
        {
            PhotonNetwork.Instantiate("Bball", transform.position, transform.rotation);
        }
    }
}
