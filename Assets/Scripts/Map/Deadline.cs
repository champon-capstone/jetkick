using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Deadline : MonoBehaviour
{
    public Camera camera;
    public PlayerCamera playerCamera;
  

    private GameManager _gameManager;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            GameObject BigExplosion;
            BigExplosion = Resources.Load("BigExplosion") as GameObject;
            Instantiate(BigExplosion, col.gameObject.transform.position, Quaternion.identity);
          
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
        _gameManager.RequestCarCountMinus();
    }
}