using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;
    #endregion

    #region Pirvate Fields

    string gameVersion = "1";
    bool isConnecting;

    #endregion

    #region Public Fields

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;

    public Button playButton;
    public Button connectButton;

    #endregion

    #region MonoBehaviour CallBacks

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        playButton.interactable = false;
    }

    #endregion

    #region Pulbic Methods

    public void Connect()
    {
        isConnecting = true;
        connectButton.interactable = false;
        playButton.interactable = true;
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Play()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    #endregion

    #region MonoBehaviourPunCallBacks Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect to Server");
        playButton.interactable = true;
        //if (isConnecting)
        //{
        //    PhotonNetwork.JoinRandomRoom();
        //}
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        playButton.interactable = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");

       PhotonNetwork.LoadLevel("Lobby");
    }

    #endregion
}
