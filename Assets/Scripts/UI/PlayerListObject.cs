using System;
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
    public Dropdown colorDropdown;

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
    }

    public void SetMasterColor()
    {
        PlayerNameText.color = Color.red;
    }
    
    public String GetPlayerColor()
    {
        return colorDropdown.options[colorDropdown.value].text;
    }

    public void SetPlayerColor(string colorString)
    {
        for (int i = 0; i < colorDropdown.options.Count; i++)
        {
            if (colorDropdown.options[i].text.Equals(colorString))
            {
                colorDropdown.value = i;
                break;
            }
        }
    }
}