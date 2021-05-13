using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deadline : MonoBehaviour
{
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
            
            Destroy(col.gameObject.transform.parent.gameObject);
            
        }
    }
}