using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Public Fields

    public const string PLAYER_LIVES = "PlayerLives";
    public const string PLAYER_READY = "IsPlayerReady";
    public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";
    public const int PLAYER_MAX_LIVES = 3;

    public GameObject camera;
    public GameObject defaultCamera;
    
    public static GameManager instance;
    
    #endregion

    private GameObject testCar;
    private string playerPrefab = "TestCar2";

    #region Photon Callbacks


    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName); // not seen if you're the player connecting
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
    }

    #endregion

    private void Start()
    {
        instance = this;
        if (PlayerManager.LocalPlayerInstance == null)
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            object playerPosition;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("position", out playerPosition);
            Vector3 test = (Vector3) playerPosition;
            testCar = PhotonNetwork.Instantiate(playerPrefab, test, Quaternion.identity, 0);
            
            camera.GetComponent<PlayerCamera>().target = testCar.transform;
            Destroy(defaultCamera);
        }
        else
        {
            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        }
    }

    #region Public Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    
    #endregion
}
