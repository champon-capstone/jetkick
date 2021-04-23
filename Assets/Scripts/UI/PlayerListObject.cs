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
    public Image PlayerColor;
    public Dropdown ColorDropdown;

    private int ownerId;
    private Dictionary<string, Color> colorMap;
    
    public void Start()
    {
        Hashtable initialProps = new Hashtable() {{GameManager.PLAYER_LIVES, GameManager.PLAYER_MAX_LIVES}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
        PhotonNetwork.LocalPlayer.SetScore(0);
        colorMap = new Dictionary<string, Color>();
        colorMap.Add("Black", Color.black);
        colorMap.Add("Blue", Color.blue);
        colorMap.Add("Red", Color.red);
        colorMap.Add("Green", Color.green);
    }

    public void Initialize(int playerId, string playerName)
    {
        ownerId = playerId;
        PlayerNameText.text = playerName;
        PlayerColor.gameObject.SetActive(true);
    }

    public void ChangeColor()
    {
        PlayerColor.color = colorMap[ColorDropdown.options[ColorDropdown.value].text];
    }
}