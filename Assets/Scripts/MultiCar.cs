using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class MultiCar : MonoBehaviour
{
    public float power = 10f;

    public GameObject missilePosition;
    public GameObject bananaPosition;
    public GameObject shieldPosition;
    
    private PhotonView _photonView;
    private Rigidbody rbody;

    private bool isShield = false;

    private bool isItemMode = false;
    
    private void Awake()
    {
        _photonView = PhotonView.Get(this);
    }

    private void Start()
    {
        rbody = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!isItemMode)
        {
            return;
        }
        if (!_photonView.IsMine)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            MissileAttack();
        }

        if (Input.GetMouseButtonDown(1))
        {
            ActiveShield();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isShield)
        {
            return;
        }
        
        if (other.gameObject.CompareTag("Missile"))
        {
            _photonView.RPC("MissileAttacked", PhotonNetwork.LocalPlayer, other);
            // PhotonNetwork.Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Banana"))
        {
            _photonView.RPC("BananaAttacked", PhotonNetwork.LocalPlayer, other);
            // PhotonNetwork.Destroy(other.gameObject);
        }
        
        // if (other.CompareTag("Player"))
        // {
        //    _photonView.RPC("BallAttacked", PhotonNetwork.LocalPlayer);
        // }
    }

    public void SetItemMode(bool mode)
    {
        isItemMode = mode;
    }

    #region Item Effect

    [PunRPC]
    private void BananaAttacked(Collider info)
    {
        rbody.AddTorque(Vector3.right * 1000000.0f);
        PhotonNetwork.Destroy(info.gameObject);
    }

    [PunRPC]
    private void MissileAttacked(Collider info)
    {
        Debug.Log("차�? 미사?�과 충돌");
        Debug.Log("Is shield "+isShield);
        if (isShield)
        {
            return;
        }
        PhotonNetwork.Instantiate("BigExplosion", info.transform.position, Quaternion.identity);
        rbody.AddForce(Vector3.up * 1000000.0f);
        PhotonNetwork.Destroy(info.gameObject);
    }

    [PunRPC]
    private void BallAttacked(Collider other)
    {
        PhotonNetwork.Instantiate("BigExplosion", other.gameObject.transform.position, Quaternion.identity);
        other.attachedRigidbody.AddForce(Vector3.left * power * 100000.0f);
        PhotonNetwork.Destroy(gameObject);
    }
    
    #endregion
    
    public void MissileAttack()
    {
        var missile = PhotonNetwork.Instantiate("missile", missilePosition.transform.position, missilePosition.transform.rotation);
    }

    public void SpawnBanana()
    {
        var banana = PhotonNetwork.Instantiate("banana", bananaPosition.transform.position, bananaPosition.transform.rotation);
    }

    public void ActiveShield()
    {
        var shield = PhotonNetwork.Instantiate("shield", shieldPosition.transform.position, shieldPosition.transform.rotation);
        shield.GetComponent<ShieldCollisionitem>().car = gameObject;
        isShield = true;
        StartCoroutine("TurnOffShield");
    }

    private IEnumerator TurnOffShield()
    {
        yield return new WaitForSeconds(3f);
        isShield = false;
    }

    public int GetActorNumber()
    {
        return _photonView.Owner.ActorNumber;
    }
}