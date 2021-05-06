using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deadline : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("자동차 사망");
            //폭발시키는 이벤트 필요
            GameObject BigExplosion;
            BigExplosion = Resources.Load("BigExplosion") as GameObject;
            Instantiate(BigExplosion, col.gameObject.transform.position, Quaternion.identity);
            Destroy(col.gameObject.transform.parent.gameObject);
            
        }
    }
}