using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ColorDropdown
{
    
    public void ChangeColorUI(GameObject content)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            var color = player.CustomProperties["color"];
            if (color == null)
            {
                return;
            }
            for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                var child = content.transform.GetChild(i);
                var playerListObject = child.GetComponent<PlayerListObject>();
                if (playerListObject.PlayerNameText.text.Equals(player.NickName))
                {
                    playerListObject.SetPlayerColor(color.ToString());
                }
            }
        }
    }
}