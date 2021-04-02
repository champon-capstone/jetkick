
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class LobbyRoomInfo : MonoBehaviour
{
    private string playerName;

    public Image playerReadyImage;
    public Button playerReadyButton;

    private int ownerID;
    private bool isPlayerReady;

    private void Start()
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber != ownerID)
        {
            playerReadyButton.gameObject.SetActive(false);
        }
        else
        {
            Hashtable initialProps = new Hashtable() { { GameManager.PLAYER_READY, isPlayerReady }, { GameManager.PLAYER_LIVES, GameManager.PLAYER_MAX_LIVES } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);

            playerReadyButton.onClick.AddListener(() =>
            {
                isPlayerReady = !isPlayerReady;
                SetPlayerReady(isPlayerReady);

                Hashtable props = new Hashtable() { { GameManager.PLAYER_READY, isPlayerReady } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                if (PhotonNetwork.IsMasterClient)
                {
                    FindObjectOfType<LobbyMain>().LocalPlayerPropertiesUpdated();
                }
            });
        }
    }

    public void Initialize(int playerID, string playerName)
    {
        ownerID = playerID;
        this.playerName = playerName; 
    }

    public void SetPlayerReady(bool playerReady)
    {
        playerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "Ready!" : "Ready?";
        playerReadyImage.enabled = playerReady;
    }
}
