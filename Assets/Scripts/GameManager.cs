using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Public Fields

    [Header("Position")] public GameObject position1;
    public GameObject position2;
    public GameObject position3;
    public GameObject position4;

    public const string PLAYER_LIVES = "PlayerLives";
    public const string PLAYER_READY = "IsPlayerReady";
    public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";
    public const int PLAYER_MAX_LIVES = 3;

    public GameObject camera;
    public GameObject defaultCamera;

    public GameObject indicator;

    public GameObject winnerPanel;
    public Text winnerText;

    public static GameManager instance;

    public Material[] colors;
    
    #endregion

    private GameObject testCar;
    private string playerPrefab = "TestCar3";
    private Dictionary<int, GameObject> positionMap;
    private Dictionary<string, Material> colorMap;

    private Dictionary<string, ArrayList> teamPlayerCount;


    private PhotonView _photonView;

    private string localPlayerColor;

    private string mode;

    private string requestAdd = "add";
    private string requestDelete = "die";

    private int totalPlayerCarCount = 0;

    private WeatherManager _weatherManager;
    

    #region Unity

    private void Awake()
    {
        _weatherManager = FindObjectOfType<WeatherManager>();
        teamPlayerCount = new Dictionary<string, ArrayList>();
        _photonView = PhotonView.Get(this);
        colorMap = new Dictionary<string, Material>();
        colorMap.Add("GREEN", colors[1]);
        colorMap.Add("RED", colors[0]);
        colorMap.Add("WHITE", colors[2]);
        positionMap = new Dictionary<int, GameObject>();
        positionMap.Add(0, position1);
        positionMap.Add(1, position2);
        positionMap.Add(2, position3);
        positionMap.Add(3, position4);


        

        winnerPanel.SetActive(false);
    }

    private void Start()
    {
        instance = this;

        totalPlayerCarCount = 0;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.MasterClient.SetCustomProperties(new Hashtable() {{"init", true}});

            object modeText;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("mode", out modeText);
            mode = (string) modeText;
            object test;
            PhotonNetwork.MasterClient.CustomProperties.TryGetValue("weather", out test);
            if (test != null)
            {
                Weather weather;
                if (Enum.TryParse(test.ToString(), out weather))
                {
                    _weatherManager.weather = weather;
                }
            }
        }
        
        if (PlayerManager.LocalPlayerInstance == null)
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
            object playerPosition;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("position", out playerPosition);
            int index = (int) playerPosition;
            object color;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("color", out color);
            object car;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("car", out car);

            if (car != null)
            {
                string carName = car.ToString();
                string[] tokens = carName.Split('(');
                testCar = PhotonNetwork.Instantiate(tokens[0]+"_1", positionMap[index].transform.position,
                    positionMap[index].transform.rotation, 0);
                PhotonNetwork.LocalPlayer.TagObject = testCar;
            }
            
            if (testCar == null)
            {
                testCar = PhotonNetwork.Instantiate("Car1_1", positionMap[index].transform.position,
                    positionMap[index].transform.rotation, 0);
                PhotonNetwork.LocalPlayer.TagObject = testCar;
            }

            if (color != null)
            {
                localPlayerColor = color.ToString();
            }
            
            camera.GetComponent<PlayerCamera>().target = testCar.transform;
            // Destroy(defaultCamera);
            defaultCamera.gameObject.SetActive(false);

            PhotonNetwork.LocalPlayer.SetCustomProperties(
                new Hashtable() {{requestAdd, 1}, {"color", localPlayerColor}});

            PhotonNetwork.LocalPlayer.TagObject = testCar;
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {{"indicator", color.ToString()}});

            object map;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("map", out map);
            
            if (map == null || !map.ToString().Equals("ObstacleMap"))
            {
                testCar.GetComponent<MultiCar>().SetItemMode(false);
            }
            else
            {
                var itemManager = FindObjectOfType<ItemManager>();
                if (itemManager != null && PhotonNetwork.LocalPlayer.IsLocal)
                {
                    itemManager.SetMultiCat(testCar.GetComponent<MultiCar>());
                }
                testCar.GetComponent<MultiCar>().SetItemMode(true);
            }
        }
        else
        {
            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        }
    }




    #endregion


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("indicator"))
        {
            var indicator = Instantiate(this.indicator, Vector3.zero, Quaternion.identity);
            var indicatorScript = indicator.GetComponent<PlayerIndicator>();
            indicatorScript.camera = camera.GetComponent<PlayerCamera>().camera;
            indicatorScript.username = targetPlayer.NickName;
            indicatorScript.color = colorMap[changedProps["indicator"].ToString()].color;
            foreach (MultiCar car in FindObjectsOfType<MultiCar>())
            {
                if (car.GetActorNumber() == targetPlayer.ActorNumber)
                {
                    indicator.transform.SetParent(car.transform);
                    indicatorScript.target = car.transform;
                    break;
                }
            }
            return;
        }

        CheckGameOver(changedProps, targetPlayer);
    }

    private void CheckGameOver(Hashtable info, Player targetPlayer)
    {
        if (mode == null)
        {
            return;
        }
        if (mode.Equals("Solo"))
        {
            SoloMode(info);
        }
        else
        {
            TeamMode(info, targetPlayer);
        }
    }

    private void TeamMode(Hashtable changedProps, Player targetPlayer)
    {
        if (!changedProps.ContainsKey("color"))
        {
            return;
        }
        var color = changedProps["color"].ToString();
       
        if (!teamPlayerCount.ContainsKey(color))
        {
            teamPlayerCount.Add(color, new ArrayList());
        }
        
        
        ChangeTeamPlayerCount(changedProps, color, teamPlayerCount, targetPlayer);
    }

    private void ChangeTeamPlayerCount(Hashtable changedProps, string color, Dictionary<string, ArrayList> teamCount, Player targetPlayer)
    {
        if (changedProps.ContainsKey(requestAdd))
        {
            Debug.Log("Add ZXC"+targetPlayer.ActorNumber);
            totalPlayerCarCount += 1;
            if (!teamCount[color].Contains(targetPlayer.ActorNumber))
            {
                teamCount[color].Add(targetPlayer.ActorNumber);       
            }
        }

        if (changedProps.ContainsKey(requestDelete))
        {
            Debug.Log("Delete ZXC"+targetPlayer.ActorNumber);
            totalPlayerCarCount -= 1;
            teamCount[color].Remove(targetPlayer.ActorNumber);
            CheckTeamGameOver(teamCount);
        }
        
        Debug.Log("ZXC CheckGameOver Team "+totalPlayerCarCount);
    }

    private void CheckTeamGameOver(Dictionary<string, ArrayList> teamPlayer)
    {
        foreach (string teamColor in teamPlayer.Keys)
        {
            if (teamPlayer[teamColor].Count > 0)
            {
                if (IsTeamGameOver(teamPlayer, teamColor))
                {
                    _photonView.RPC("DisplayTeamWinner", RpcTarget.All, teamColor);
                }
            }
        }
    }

    private bool IsTeamGameOver(Dictionary<string, ArrayList> teamPlayer, string targetColor)
    {
        foreach (string teamPlayerKey in teamPlayer.Keys)
        {
            if (!teamPlayerKey.Equals(targetColor))
            {
                if (teamPlayer[teamPlayerKey].Count > 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void SoloMode(Hashtable changedProps)
    {
        if (changedProps.ContainsKey(requestAdd))
        {
            totalPlayerCarCount += (int) changedProps[requestAdd];
            Debug.Log("count add " + totalPlayerCarCount);
        }

        if (changedProps.ContainsKey(requestDelete))
        {
            totalPlayerCarCount += (int) changedProps[requestDelete];
            Debug.Log("count die " + totalPlayerCarCount);
            if (totalPlayerCarCount <= 1)
            {
                _photonView.RPC("DisplayWinner", RpcTarget.All);
                Debug.Log("GameOver");
            }
        }
        
        Debug.Log("CheckGameOver Solo "+totalPlayerCarCount);
    }

    [PunRPC]
    private void DisplayTeamWinner(string color)
    {
        winnerText.text = color + " Team";
        winnerPanel.SetActive(true);
    }

    [PunRPC]
    private void DisplayWinner()
    {
        var list = FindObjectsOfType<MultiCar>();
        string text = "";
        foreach (MultiCar player in list)
        {
            text += player.gameObject.GetComponent<PhotonView>().Owner.NickName + "\n";
        }

        winnerText.text = text;
        winnerPanel.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.Log("Player left room "+other.NickName);
        GameObject testObject = (GameObject) other.TagObject;
        if (testObject != null)
        {
            PhotonNetwork.Destroy(testObject);
        }

        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName);
    }


    public void LeaveRoom()
    {
        Debug.Log("��ư����");
        if (PlayerManager.LocalPlayerInstance != null)
        {
            RequestCarCountMinus();
        }

        PhotonNetwork.LeaveRoom();
    }

    public void RequestCarCountMinus()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(
            new Hashtable() {{requestDelete, -1}, {"color", localPlayerColor}});
    }
}