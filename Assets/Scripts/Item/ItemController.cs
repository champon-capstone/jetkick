using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{

    public GameObject effect;
    public float respawnTime;
    private float spinspeed = 20.0f;
    Renderer renderer;

    private ItemManager _itemManager;
    
    // Start is called before the first frame update
    void Start()
    {
        renderer = this.GetComponentInChildren<Renderer>();
        _itemManager = FindObjectOfType<ItemManager>();
    }

    // Update is called once per frame
    void Update()
    {
        float y = spinspeed * Time.deltaTime;
        transform.Rotate(Vector3.up * y);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.gameObject.GetComponent<MultiCar>().GetActorNumber() != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }
        
        if(other.gameObject.tag =="Player")
        {
            Debug.Log("Local Player "+PhotonNetwork.LocalPlayer.NickName+" item manager owner "+_itemManager.multiCar._photonView.Owner.NickName);
            Instantiate(effect, this.transform);
            //plusItem();
            _itemManager.plusItem();
            StartCoroutine("FadeOut");
         
            Invoke("respawn", respawnTime);
        }
    }

    void respawn()
    {
        StartCoroutine("FadeIn");
    }


    IEnumerator FadeOut()
    {
        int i = 10;
        while (i > 0)
        {
            i -= 1;
            float f = i / 10.0f;
            Color c = renderer.material.color;
            c.a = f;
            renderer.material.color = c;
            yield return new WaitForSeconds(0.02f);
        }
    }

    IEnumerator FadeIn()
    {
        int i = 0;
        while (i < 10)
        {
            i += 1;
            float f = i / 10.0f;
            Color c = renderer.material.color;
            c.a = f;
            renderer.material.color = c;
            yield return new WaitForSeconds(0.02f);
        }
    }

    
}
