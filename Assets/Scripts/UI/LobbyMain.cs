using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyMain : MonoBehaviourPunCallbacks
{

    #region Public Fields

    public GameObject roomListObject;
    public GameObject roomList;
    public Button StartButton;
    public GameObject roomPanel;
    public GameObject createPanel;

    #endregion

    #region Private Fields

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<string, GameObject> playerListEntries;
    private Dictionary<string, GameObject> panelList;

    #endregion



    #region Unity

    private void Awake()
    {
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();
        panelList = new Dictionary<string, GameObject>();
        roomPanel.SetActive(false);
    }

    private void Start()
    {
        panelList.Add("roomPanel", roomPanel);
        panelList.Add("createPanel", createPanel);
    }

    #endregion

    #region Pun CallBacks

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }


    #endregion

    private void ClearRoomListView()
    {
        foreach(GameObject room in roomListEntries.Values)
        {
            Destroy(room);
        }

        roomListEntries.Clear();
    }

    private void UpdateCachedRoomList(List<RoomInfo> roominfos)
    {
        foreach(RoomInfo info in roominfos)
        {
            if(!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }
                continue;
            }
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    private void UpdateRoomListView()
    {
        foreach(RoomInfo info in cachedRoomList.Values)
        {
            var room = Instantiate(roomListObject);
            room.transform.SetParent(roomList.transform);
            room.transform.localScale = Vector3.one;
            room.GetComponent<LobbyRoomInfo>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

            roomListEntries.Add(info.Name, room);
        }
    }

    public void LocalPlayerPropertiesUpdated()
    {
        StartButton.gameObject.SetActive(CheckPlayersReady());
    }

    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(GameManager.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public void ActivePanel(string panelName)
    {
        panelList[panelName].SetActive(true);
    }

    public void OnCreateRoomButtonClicked()
    {

    }
}
