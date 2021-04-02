using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyMain : MonoBehaviourPunCallbacks
{

    #region Public Fields

    public Button StartButton;
    [Header("List Panel")]
    public GameObject listPanel;
    public GameObject roomListObject;
    [Header("Room Panel")]
    public GameObject roomPanel;
    [Header("Create Room")]
    public GameObject createPanel;
    public InputField roonNameInput;
    public InputField playerNumberInput;


    #endregion

    #region Private Fields

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<string, GameObject> playerListEntries;
    private Dictionary<string, GameObject> panelList;

    private string currentPanel;
    private string beforePanel;

    private const string roomPanelName = "roomPanel";
    private const string createPanelName = "createPanel";
    private const string listPanelName = "listPanel";

    #endregion



    #region Unity

    private void Awake()
    {
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();
        panelList = new Dictionary<string, GameObject>();
        roomPanel.SetActive(false);
        createPanel.SetActive(false);
        currentPanel = listPanelName;
        beforePanel = listPanelName;
    }

    private void Start()
    {
        panelList.Add(roomPanelName, roomPanel);
        panelList.Add(createPanelName, createPanel);
        panelList.Add(listPanelName, listPanel);
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
            room.transform.SetParent(listPanel.transform);
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
        panelList[beforePanel].SetActive(false);
        beforePanel = currentPanel;
        panelList[panelName].SetActive(true);
        currentPanel = panelName;
    }

    private void OnCreateRoomButtonClicked()
    {
        Debug.Log("Request Create Room");
        string roomName = roonNameInput.text;

        byte maxPlayer;
        byte.TryParse(playerNumberInput.text,out maxPlayer);
        maxPlayer = (byte)Mathf.Clamp(maxPlayer, 1, 4);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayer, PlayerTtl = 10000 };

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public void OnOkButtonClicked()
    {
        if(currentPanel == null)
        {
            return;
        }

        if (currentPanel.Equals(createPanelName))
        {
            OnCreateRoomButtonClicked();
            panelList[currentPanel].SetActive(false);
            currentPanel = roomPanelName;
            ActivePanel(currentPanel);
        }
    }
}
