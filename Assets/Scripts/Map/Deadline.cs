using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Deadline : MonoBehaviour
{
    public Camera camera;
    public PlayerCamera playerCamera;
    private CameraTest cameraManager;
    // Start is called before the first frame update
    void Start()
    {
        cameraManager = GetComponent<CameraTest>();
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            GameObject BigExplosion;
            BigExplosion = Resources.Load("BigExplosion") as GameObject;
            Instantiate(BigExplosion, col.gameObject.transform.position, Quaternion.identity);
            if (cameraManager != null)
            {
                Debug.Log("cameraManger test");
                cameraManager.Carcamera.transform.parent.parent.gameObject.GetComponent<PlayerCamera>().enabled = false;
                Destroy(cameraManager.Carcamera.transform.parent.parent.gameObject);
                cameraManager.Cardead();
            }
        }

        if (col == null || col.transform.parent == null)
        {
            return;
        }
        
        if (PhotonNetwork.LocalPlayer.ActorNumber == col.transform.parent.GetComponent<MultiCar>().GetActorNumber())
        {
            playerCamera.target = camera.transform;
        }
        PhotonNetwork.Destroy(col.transform.parent.gameObject);
    }
}