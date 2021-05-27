using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ColorDropdown
{
    public GameObject content;

    public void ChangeColorUI()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            var color = player.CustomProperties["color"];
            for (int i = 0; i < PhotonNetwork.CountOfPlayersInRooms; i++)
            {
                var child = content.transform.GetChild(i);
                var playerListObject = child.GetComponent<PlayerListObject>();
                if (playerListObject.name.Equals(player.NickName))
                {
                    playerListObject.SetPlayerColor(color.ToString());
                }
                if (playerListObject.name.Equals(PhotonNetwork.LocalPlayer.NickName))
                {   
                    
                }
            }
        }
    }
}