
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class PlayerListObject : MonoBehaviour
{

    [Header("UI References")]
    public Text PlayerNameText;

    public Button PlayerReadyButton;
    public Image PlayerReadyImage;

    private int ownerId;
    private bool isPlayerReady;


    public void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
        {
            PlayerReadyButton.gameObject.SetActive(false);
        }
        else
        {
            Hashtable initialProps = new Hashtable() { { GameManager.PLAYER_READY, isPlayerReady }, { GameManager.PLAYER_LIVES, GameManager.PLAYER_MAX_LIVES } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
            PhotonNetwork.LocalPlayer.SetScore(0);

            PlayerReadyButton.onClick.AddListener(() =>
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

    public void Initialize(int playerId, string playerName)
    {
        ownerId = playerId;
        PlayerNameText.text = playerName;
    }

    public void SetPlayerReady(bool playerReady)
    {
        PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "Ready!" : "Ready?";
        PlayerReadyImage.enabled = playerReady;
    }
}
