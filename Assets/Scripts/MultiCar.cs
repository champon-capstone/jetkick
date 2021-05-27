using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MultiCar : MonoBehaviour
{
    
    private PhotonView _photonView;
    private bool isShield;
    
    private void Start()
    {
        isShield = false;
        _photonView = PhotonView.Get(this);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _photonView.RPC("Attacked", RpcTarget.Others);
        }

        if (Input.GetMouseButtonDown(1))
        {
            isShield = true;
        }
    }

    [PunRPC]
    private void Attacked(PhotonMessageInfo info)
    {
        Debug.Log("Shield "+isShield);
        if (isShield)
        {
            Debug.Log("Shield in "+info.Sender.NickName + info.photonView);
            return;
        }
        GetComponent<Rigidbody>().AddForce(Vector3.forward * 3000);
        Debug.Log("Attacked Sender  "+info.Sender.NickName + "Receiver "+PhotonNetwork.LocalPlayer.NickName);
    }

}
