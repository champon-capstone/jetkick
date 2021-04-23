using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyMain : MonoBehaviourPunCallbacks
{

    #region Public Fields

    public Button StartButton;
    [Header("List Panel")]
    public GameObject listPanel;
    public GameObject roomListPrefab;
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

    private int listDelta = 0;
    private int roomDelta = 0;
    private Vector3 defaultRoomPosition;
    private Vector3 defaultPlayerPosition;

    #endregion

    private Vector3 testPosition1 = new Vector3(-132.4f, 40f, -160f);
    private Vector3 testPosition2 = new Vector3(-132.4f, 40f, -130f);
    
    

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
        defaultRoomPosition = new Vector3(listPanel.transform.position.x, listPanel.transform.position.y + 80, listPanel.transform.position.z);
        defaultPlayerPosition = new Vector3(roomPanel.transform.position.x, roomPanel.transform.position.y + 80, roomPanel.transform.position.z);
    }

    #endregion

    #region Pun CallBacks

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate roomlist size"+roomList.Count);
        foreach(RoomInfo info in roomList)
        {
            Debug.Log("Room Info " + info.Name);
        }
        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnJoinedLobby()
    {
        cachedRoomList.Clear();
        ClearRoomListView();
        Debug.Log("Joined Lobby");
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
        Debug.Log("OnJoinedRoom");
        cachedRoomList.Clear();

        ActivePanel(roomPanel.name);
        panelList[createPanel.name].SetActive(false);
        panelList[listPanel.name].SetActive(false);

        if(playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        UpdatePlayerList();

        //StartButton.gameObject.SetActive(CheckPlayersReady());


        Hashtable props = new Hashtable
            {
                {GameManager.PLAYER_LOADED_LEVEL, false}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnLeftRoom()
    {
        roomDelta = 0;
        listDelta = 0;
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
        Debug.Log("OnPlayerEneteredRoom");
        listDelta += 20;

        GameObject entry = Instantiate(playerListObject);
        entry.transform.SetParent(roomPanel.transform);
        entry.transform.localScale = Vector3.one;
        var playerPosition = new Vector3(defaultRoomPosition.x, defaultRoomPosition.y - listDelta, defaultRoomPosition.z);
        entry.transform.position = playerPosition;
        entry.GetComponent<PlayerListObject>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);
        
        
        playerListEntries.Add(newPlayer.ActorNumber, entry);

        //StartButton.gameObject.SetActive(CheckPlayersReady());
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

        listDelta = 0;
        
        UpdatePlayerList();
        
        roomDelta += 20;
        //StartButton.gameObject.SetActive(CheckPlayersReady());
    }



    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            //StartButton.gameObject.SetActive(CheckPlayersReady());
        }
    }

    
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        GameObject entry;
        // if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        // {
        //     object isPlayerReady;
        //     if (changedProps.TryGetValue(GameManager.PLAYER_READY, out isPlayerReady))
        //     {
        //         entry.GetComponent<PlayerListObject>().SetPlayerReady((bool)isPlayerReady);
        //     }
        // }

        //StartButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        var startFlag = propertiesThatChanged["Start"];
        if (startFlag != null)
        {
            PhotonNetwork.LoadLevel("TestMap");
        }
    }
    
    #endregion

    public void OnLeaveRoomButtonClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        panelList[roomPanel.name].SetActive(false);
        StartButton.gameObject.SetActive(true);
    }

    private void UpdatePlayerList()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject playerObject = Instantiate(playerListObject);
            playerObject.transform.SetParent(roomPanel.transform);
            playerObject.transform.localScale = Vector3.one;
            var playerPosition =
                new Vector3(defaultRoomPosition.x, defaultRoomPosition.y - listDelta, defaultRoomPosition.z);
            playerObject.transform.position = playerPosition;
            playerObject.GetComponent<PlayerListObject>().Initialize(p.ActorNumber, p.NickName);
            listDelta += 20;
            
            Debug.Log("Player "+p.NickName+" Position "+playerObject.transform.position);

            //TODO Change Player Position
            if (p.NickName.Equals("kbh"))
            {
                p.CustomProperties.Add("position", testPosition1);
            }
            else
            {
                p.CustomProperties.Add("position", testPosition2);
            }
            
            // object isPlayerReady;
            // if (p.CustomProperties.TryGetValue(GameManager.PLAYER_READY, out isPlayerReady))
            // {
            //     playerObject.GetComponent<PlayerListObject>().SetPlayerReady((bool) isPlayerReady);
            // }

            playerListEntries.Add(p.ActorNumber, playerObject);
        }
    }
    
    private void ClearRoomListView()
    {
        listDelta = 0;
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

    public void UpdateRoomListView()
    {
        foreach(RoomInfo info in cachedRoomList.Values)
        {
            roomDelta -= 20;
            var room = Instantiate(roomListPrefab);
            room.transform.SetParent(listPanel.transform);
            room.transform.localScale = Vector3.one;
            var roomPosition = new Vector3(defaultRoomPosition.x, defaultRoomPosition.y - roomDelta, defaultRoomPosition.z);
            room.transform.position = roomPosition;
            room.GetComponent<LobbyRoomInfo>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

            roomListEntries.Add(info.Name, room);
        }
    }

    public void LocalPlayerPropertiesUpdated()
    {
        //StartButton.gameObject.SetActive(CheckPlayersReady());
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
        currentPanel = panelName;
    }

    private void OnCreateRoomButtonClicked()
    {
        string roomName = roonNameInput.text;

        byte maxPlayer;
        byte.TryParse(playerNumberInput.text,out maxPlayer);
        maxPlayer = (byte)Mathf.Clamp(maxPlayer, 1, 8);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayer, PlayerTtl = 10000, IsVisible = true };

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    private void StartGame()
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() {{"Start", true}});
    }
    
    public void OnOkButtonClicked()
    {
        if(currentPanel == null)
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
            OnLeaveRoomButtonClicked();
        }
    }

}
