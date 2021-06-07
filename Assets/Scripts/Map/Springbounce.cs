using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Springbounce : MonoBehaviour
{
	// Start is called before the first frame update
	public float force = 1000f; //Force 10000f
	
	private Vector3 hitDir;
	private Rigidbody rb;


    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            //Debug.Log("Æ¨±â´ÂÁß");
			other.attachedRigidbody.AddForce(new Vector3(0.0f,force,0.0f) * 100.0f);
        }
    }

}
