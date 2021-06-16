using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LobbyMain : MonoBehaviourPunCallbacks
{
    #region Public Fields

    public Button StartButton;
    [Header("List Panel")] public GameObject listPanel;
    public GameObject roomListPanel;
    public GameObject roomListPrefab;
    [Header("Room Panel")] public GameObject roomPanel;
    public GameObject playerListPanel;
    public GameObject playerListObject;
    [Header("Create Room")] public GameObject createPanel;
    public InputField roonNameInput;
    public InputField playerNumberInput;
    public Dropdown mapDropdown;
    public Dropdown modeDropdown;
    public Dropdown weatherDropdown;

    

    [Header("Commom UI")] public Text roomName;
    public Image mapImage;
    public Text mapName;
    public Text mapDescription;
    public Chat chat;
    public GameObject uitlButtonPanel;

    #endregion

    #region Private Fields

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;
    private Dictionary<string, GameObject> panelList;
    private Dictionary<string, MapInfo> mapInfoDic;

    private GameObject localPlayer;

    private ColorDropdown dropdown = new ColorDropdown();

    private string currentPanel;

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
    }

    private void Start()
    {
        panelList.Add(roomPanel.name, roomPanel);
        panelList.Add(createPanel.name, createPanel);
        panelList.Add(listPanel.name, listPanel);
        mapInfoDic = new Dictionary<string, MapInfo>();

        var generator = new MapInfoGenerator();
        foreach (MapInfo info in generator.MapList)
        {
            mapInfoDic.Add(info.MapName, info);
        }

        roomName.gameObject.SetActive(false);
        mapImage.gameObject.SetActive(false);
        mapName.gameObject.SetActive(false);
        mapDescription.gameObject.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
    }

    #endregion

    #region Pun CallBacks

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            Debug.Log("Room Info " + info.Name);
        }

        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Join Lobby");
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Left Lobby");
        // cachedRoomList.Clear();
        // ClearRoomListView();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.ReconnectAndRejoin();
        Debug.Log("Join Falied " + returnCode + " message " + message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string roomName = "Room " + Random.Range(1000, 10000);

        RoomOptions options = new RoomOptions {MaxPlayers = 4};

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");

        chat.ConnectToRoomChat(roomName.text);

        // cachedRoomList.Clear();

        roomName.gameObject.SetActive(true);


        ActivePanel(roomPanel.name);
        panelList[createPanel.name].SetActive(false);
        panelList[listPanel.name].SetActive(false);

        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        Hashtable props;

        if (PhotonNetwork.IsMasterClient)
        {
            props = new Hashtable
            {
                {GameManager.PLAYER_LOADED_LEVEL, false},
                {"isMaster", true},
                {"color", "GREEN"}
            };
        }
        else
        {
            props = new Hashtable
            {
                {GameManager.PLAYER_LOADED_LEVEL, false},
                {"isMaster", false},
                {"color", "GREEN"}
            };
        }


        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        UpdatePlayerList();
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
        GameObject entry = Instantiate(playerListObject, playerListPanel.transform);
        
        var listObject = entry.GetComponent<PlayerListObject>();
        
        listObject.Initialize(newPlayer.ActorNumber, newPlayer.NickName);
        listObject.colorDropdown.onValueChanged.AddListener(delegate(int arg0) { UpdateColor(); });
        
        var objectName = listObject.PlayerNameText.text;
        var localName = PhotonNetwork.LocalPlayer.NickName;
        if (!localName.Equals(objectName))
        {
            listObject.UnInteractive();
        }
        
        playerListEntries.Add(newPlayer.ActorNumber, entry);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);

        foreach (var playerObject in playerListEntries.Values)
        {
            Destroy(playerObject);
        }

        playerListEntries.Clear();


        UpdatePlayerList();
    }


    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == newMasterClient.ActorNumber)
            {
                player.SetCustomProperties(new Hashtable() {{"isMaster", true}});
                break;
            }
        }

        ChangeMasterClientColor();
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        if (changedProps.ContainsKey("color"))
        {
            dropdown.ChangeColorUI(playerListPanel);
        }
        
        if (currentPanel.Equals(roomPanel.name))
        {
            ChangeMasterClientColor();
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        var startFlag = propertiesThatChanged["Start"];
        if (startFlag != null)
        {
            PhotonNetwork.LoadLevel(mapDropdown.options[mapDropdown.value].text);
        }
    }

    #endregion

    #region Private Methods

    private void ChangeMasterClientColor()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties["isMaster"] != null)
            {
                var isMaster = (bool) player.CustomProperties["isMaster"];
                if (isMaster)
                {
                    playerListEntries[player.ActorNumber].GetComponent<PlayerListObject>().SetMasterColor();
                    break;
                }
            }
        }
    }

    private void UpdatePlayerList()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject playerObject = Instantiate(playerListObject, playerListPanel.transform);
            var listObject = playerObject.GetComponent<PlayerListObject>();
            listObject.Initialize(p.ActorNumber, p.NickName);

            if (localPlayer == null)
            {
                if (PhotonNetwork.LocalPlayer.NickName.Equals(p.NickName))
                {
                    localPlayer = playerObject;
                    listObject.colorDropdown.onValueChanged.AddListener(delegate(int arg0) { UpdateColor(); });
                }
            }

            var objectName = listObject.PlayerNameText.text;
            var localName = PhotonNetwork.LocalPlayer.NickName;
            if (!localName.Equals(objectName))
            {
                listObject.UnInteractive();
            }
            
            playerListEntries.Add(p.ActorNumber, playerObject);
        }
    }

    private void ClearRoomListView()
    {
        foreach (GameObject room in roomListEntries.Values)
        {
            Destroy(room);
        }

        roomListEntries.Clear();
    }

    private void UpdateCachedRoomList(List<RoomInfo> roominfos)
    {
        foreach (RoomInfo info in roominfos)
        {
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
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

    private void OnCreateRoomButtonClicked()
    {
        string roomNameText = roonNameInput.text;

        byte maxPlayer;
        byte.TryParse(playerNumberInput.text, out maxPlayer);
        maxPlayer = (byte) Mathf.Clamp(maxPlayer, 2, 8);

        RoomOptions options = new RoomOptions {MaxPlayers = maxPlayer, PlayerTtl = 10000, IsVisible = true};

        var mode = modeDropdown.options[modeDropdown.value].text;
        var weather = weatherDropdown.options[weatherDropdown.value].text;

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable()
        {
            {"weather", weather},
            {"mode", mode},
            {"map", mapName.text}    
        });
        
        
        PhotonNetwork.CreateRoom(roomNameText, options, null);

        roomName.text = roomNameText;
    }

    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int index = 0;
            var list = PhotonNetwork.PlayerList;
            for (int i = 0; i < list.Length; i++)
            {
                list[i].SetCustomProperties(new Hashtable() {{"position", index++}});
            }
            
            
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() {{"Start", true}});
        }

        
    }

    private void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        panelList[roomPanel.name].SetActive(false);
        StartButton.gameObject.SetActive(true);
    }

    private void LeaveLobby()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.LoadLevel("Launcher");
        PhotonNetwork.Disconnect();
    }

    #endregion

    public void UpdateColor()
    {
        Debug.Log("Change color");
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable()
        {
            {
                "color",
                localPlayer.GetComponent<PlayerListObject>().GetPlayerColor()
            }
        });
        
    }

    public void UpdateRoomListView()
    {
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            if (!roomListEntries.ContainsKey(info.Name))
            {
                var room = Instantiate(roomListPrefab, roomListPanel.transform);
                room.GetComponent<LobbyRoomInfo>().Initialize(info.Name, (byte) info.PlayerCount, info.MaxPlayers);

                roomListEntries.Add(info.Name, room);
            }
        }
    }

    public void LocalPlayerPropertiesUpdated()
    {
        //StartButton.gameObject.SetActive(CheckPlayersReady());
    }

    public void ActivePanel(string panelName)
    {
        if (panelName.Equals(listPanel.name))
        {
            uitlButtonPanel.gameObject.SetActive(true);
        }
        else
        {
            uitlButtonPanel.gameObject.SetActive(false);
        }

        if (panelName.Equals(createPanel.name))
        {
            mapName.text = mapInfoDic["CityMap"].MapName;
            mapDescription.text = mapInfoDic["CityMap"].MapDescription;
            mapImage.sprite = mapInfoDic["CityMap"].MapImage;
            mapImage.gameObject.SetActive(true);
            mapName.gameObject.SetActive(true);
            mapDescription.gameObject.SetActive(true);
            listPanel.gameObject.SetActive(false);
        }

        panelList[panelName].SetActive(true);
        currentPanel = panelName;
    }

    public void OnMapDropdownValueChanged()
    {
        var selectedMapName = mapDropdown.options[mapDropdown.value].text;
        if (mapInfoDic.ContainsKey(selectedMapName))
        {
            mapName.text = mapInfoDic[selectedMapName].MapName;
            mapDescription.text = mapInfoDic[selectedMapName].MapDescription;
            mapImage.sprite = mapInfoDic[selectedMapName].MapImage;
        }
    }

    public void OnOkButtonClicked()
    {
        if (currentPanel == null)
        {
            return;
        }

        if (currentPanel.Equals(roomPanel.name))
        {
            StartGame();
        }

        if (currentPanel.Equals(createPanel.name))
        {
            OnCreateRoomButtonClicked();
            panelList[currentPanel].SetActive(false);
            currentPanel = roomPanel.name;
            ActivePanel(currentPanel);
        }
    }

    public void OnCancelButtonClicked()
    {
        if (currentPanel.Equals(roomPanel.name))
        {
            LeaveRoom();
            chat.ConnectToLobby();
        }
        else if (currentPanel.Equals(createPanel.name))
        {
            panelList[currentPanel].SetActive(false);
            currentPanel = listPanel.name;
            ActivePanel(currentPanel);
        }
        else if (currentPanel.Equals(listPanel.name))
        {
            LeaveLobby();
        }
    }
}