using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListObject : MonoBehaviour
{
    [Header("UI References")] 
    public Text PlayerNameText;
    public Image PlayerColor;

    private int ownerId;

    public void Start()
    {
        Hashtable initialProps = new Hashtable() {{GameManager.PLAYER_LIVES, GameManager.PLAYER_MAX_LIVES}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
        PhotonNetwork.LocalPlayer.SetScore(0);
    }

    public void Initialize(int playerId, string playerName)
    {
        ownerId = playerId;
        PlayerNameText.text = playerName;
        PlayerColor.gameObject.SetActive(true);
    }
}