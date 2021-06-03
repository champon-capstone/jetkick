using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Deadline : MonoBehaviour
{
    public Camera camera;
    public PlayerCamera playerCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            GameObject BigExplosion;
            BigExplosion = Resources.Load("BigExplosion") as GameObject;
            Instantiate(BigExplosion, col.gameObject.transform.position, Quaternion.identity);
          
        }

        if (PhotonNetwork.LocalPlayer.ActorNumber == col.transform.parent.GetComponent<MultiCar>().GetActorNumber())
        {
            playerCamera.target = camera.transform;
        }
        PhotonNetwork.Destroy(col.transform.parent.gameObject);
    }
}