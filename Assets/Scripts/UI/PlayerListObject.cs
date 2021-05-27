using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListObject : MonoBehaviour
{
    [Header("UI References")] 
    public Text PlayerNameText;
    public Dropdown teamSelector;
    public Dropdown colorDropdown;

    private int ownerId;
    private Dictionary<string, Color> colorMap;
    
    public void Start()
    {
        Hashtable initialProps = new Hashtable() {{GameManager.PLAYER_LIVES, GameManager.PLAYER_MAX_LIVES}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
        PhotonNetwork.LocalPlayer.SetScore(0);
        colorMap = new Dictionary<string, Color>();
        colorMap.Add("BLACK", Color.black);
        colorMap.Add("WHITE", Color.white);
        colorMap.Add("RED", Color.red);
    }

    public void Initialize(int playerId, string playerName)
    {
        ownerId = playerId;
        PlayerNameText.text = playerName;
    }

    public void SetMasterColor()
    {
        PlayerNameText.color = Color.red;
    }
    
    public Color GetPlayerColor()
    {
        return colorMap[colorDropdown.options[colorDropdown.value].text];
    }
}