using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool IsFiring;
    public float Health = 1f;
    public static GameObject LocalPlayerInstance;

    #region Private Fields

    #endregion

    #region MonoBehaviour CallBacks

    private void Awake()
    {
        if (photonView.IsMine)
        {
            PlayerManager.LocalPlayerInstance = this.gameObject;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        CameraWork cameraWork = this.gameObject.GetComponent<CameraWork>();
        if (cameraWork != null)
        {
            if (photonView.IsMine)
            {
                cameraWork.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }
#if UNITY_5_4_OR_NEWER
        // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        };
#endif
    }
#if !UNITY_5_4_OR_NEWER
/// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
void OnLevelWasLoaded(int level)
{
    this.CalledOnLevelWasLoaded(level);
}
#endif


    void CalledOnLevelWasLoaded(int level)
    {
        // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {

            if (Health <= 0f)
            {
                GameManager.instance.LeaveRoom();
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

        }
        else
        {
            // Network player, receive data
            this.IsFiring = (bool)stream.ReceiveNext();
            this.Health = (float)stream.ReceiveNext();
        }

    }

    #endregion

}

