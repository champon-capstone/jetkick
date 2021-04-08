using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class LobbyMain : MonoBehaviourPunCallbacks
{

    #region Public Fields

    public Button StartButton;
    [Header("List Panel")]
    public GameObject listPanel;
    public GameObject roomListObject;
    [Header("Room Panel")]
    public GameObject roomPanel;
    public GameObject playerListObject;
    [Header("Create Room")]
    public GameObject createPanel;
    public InputField roonNameInput;
    public InputField playerNumberInput;


    #endregion

    #region Private Fields

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;
    private Dictionary<string, GameObject> panelList;

    private string currentPanel;
    private string beforePanel;

    #endregion



    #region Unity

    private void Awake()
    {
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();
        panelList = new Dictionary<string, GameObject>();
        roomPanel.SetActive(false);
        createPanel.SetActive(false);
        currentPanel = listPanel.name;
        beforePanel = listPanel.name;
    }

    private void Start()
    {
        panelList.Add(roomPanel.name, roomPanel);
        panelList.Add(createPanel.name, createPanel);
        panelList.Add(listPanel.name, listPanel);
    }

    #endregion

    #region Pun CallBacks

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {


        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnJoinedLobby()
    {
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join Falied");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string roomName = "Room " + Random.Range(1000, 10000);

        RoomOptions options = new RoomOptions { MaxPlayers = 4 };

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public override void OnJoinedRoom()
    {
        cachedRoomList.Clear();

        ActivePanel(roomPanel.name);

        if(playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        foreach(Player p in PhotonNetwork.PlayerList)
        {
            GameObject playerObject = Instantiate(playerListObject);
            playerObject.transform.SetParent(roomPanel.transform);
            playerObject.transform.localScale = Vector3.one;
            playerObject.GetComponent<PlayerListObject>().Initialize(p.ActorNumber, p.NickName);


            object isPlayerReady;
            if(p.CustomProperties.TryGetValue(GameManager.PLAYER_READY, out isPlayerReady))
            {
                playerObject.GetComponent<PlayerListObject>().SetPlayerReady((bool)isPlayerReady);
            }

            playerListEntries.Add(p.ActorNumber, playerObject);
        }

        StartButton.gameObject.SetActive(CheckPlayersReady());


        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                {GameManager.PLAYER_LOADED_LEVEL, false}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnLeftRoom()
    {
        ActivePanel(listPanel.name);

        foreach (GameObject entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        playerListEntries.Clear();
        playerListEntries = null;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Enter Test "+newPlayer.NickName);

        GameObject entry = Instantiate(playerListObject);
        entry.transform.SetParent(roomPanel.transform);
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<PlayerListObject>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        playerListEntries.Add(newPlayer.ActorNumber, entry);

        StartButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);

        StartButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartButton.gameObject.SetActive(CheckPlayersReady());
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        GameObject entry;
        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(GameManager.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerListObject>().SetPlayerReady((bool)isPlayerReady);
            }
        }

        StartButton.gameObject.SetActive(CheckPlayersReady());
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

        if (currentPanel.Equals(createPanel.name))
        {
            OnCreateRoomButtonClicked();
            panelList[currentPanel].SetActive(false);
            currentPanel = roomPanel.name;
            ActivePanel(currentPanel);
        }
    }
}
