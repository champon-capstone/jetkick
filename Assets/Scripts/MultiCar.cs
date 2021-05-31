using Photon.Pun;
using UnityEngine;

public class MultiCar : MonoBehaviour
{
    private PhotonView _photonView;
    private bool isShield;
    private Rigidbody rbody;

    private void Awake()
    {
        _photonView = PhotonView.Get(this);
    }

    private void Start()
    {
        isShield = false;
        rbody = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _photonView.RPC("Attacked", RpcTarget.Others);
        }

        if (Input.GetMouseButtonDown(1))
        {
            isShield = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Missile")
        {
            _photonView.RPC("MissileAttacked", PhotonNetwork.LocalPlayer, other.transform.position);
            PhotonNetwork.Destroy(other.transform.parent.gameObject);
        }

        if (other.gameObject.tag == "Banana")
        {
            _photonView.RPC("BananaAttacked", PhotonNetwork.LocalPlayer);
            PhotonNetwork.Destroy(other.gameObject);
        }
    }

    #region Item Effect

    [PunRPC]
    private void Attacked(PhotonMessageInfo info)
    {
        Debug.Log("Shield " + isShield);
        if (isShield)
        {
            Debug.Log("Shield in " + info.Sender.NickName + info.photonView);
            return;
        }

        GetComponent<Rigidbody>().AddForce(Vector3.forward * 3000);
        Debug.Log("Attacked Sender  " + info.Sender.NickName + "Receiver " + PhotonNetwork.LocalPlayer.NickName);
    }

    [PunRPC]
    private void BananaAttacked()
    {
        rbody.AddTorque(Vector3.right * 1000000.0f);
    }

    [PunRPC]
    private void MissileAttacked(Vector3 position)
    {
        Debug.Log("차와 미사일과 충돌");
        PhotonNetwork.Instantiate("BigExplosion", position, Quaternion.identity);
        rbody.AddForce(Vector3.up * 1000000.0f);
    }
    #endregion
    
   

    public int GetActorNumber()
    {
        return _photonView.Owner.ActorNumber;
    }
}