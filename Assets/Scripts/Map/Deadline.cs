using System.Collections;
using System.Collections.Generic;
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
            cameraManager.Cardead();
            
            Destroy(col.gameObject.transform.parent.gameObject);
            camera.gameObject.SetActive(true);
            playerCamera.target = camera.transform;
        }
    }
}