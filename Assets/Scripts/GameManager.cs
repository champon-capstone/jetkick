using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    #endregion

    private GameObject testCar;
    private string playerPrefab = "TestCar3";
    private Dictionary<int, GameObject> positionMap;
    private Dictionary<string, Color> colorMap;
    private Dictionary<string, string> testMap;

    private Dictionary<string, int> teamPlayerCount;


    private PhotonView _photonView;

    private string localPlayerColor;

    private string mode;

    private string requestAdd = "add";
    private string requestDelete = "die";

    private int totalPlayerCarCount = 0;


    #region Unity

    private void Awake()
    {
        teamPlayerCount = new Dictionary<string, int>();
        _photonView = PhotonView.Get(this);
        colorMap = new Dictionary<string, Color>();
        colorMap.Add("GREEN", Color.green);
        colorMap.Add("RED", Color.red);
        colorMap.Add("WHITE", Color.white);
        positionMap = new Dictionary<int, GameObject>();
        positionMap.Add(0, position1);
        positionMap.Add(1, position2);
        positionMap.Add(2, position3);
        positionMap.Add(3, position4);
        testMap = new Dictionary<string, string>();
        testMap.Add("WHITE", "TestCar3_white");
        testMap.Add("RED", "TestCar3_red");
        testMap.Add("GREEN", "TestCar3_green");


        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            PhotonNetwork.MasterClient.SetCustomProperties(new Hashtable() {{"init", true}});

            object modeText;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("mode", out modeText);
            mode = (string) modeText;
        }

        winnerPanel.SetActive(false);
    }

    private void Start()
    {
        instance = this;

        totalPlayerCarCount = 0;

        Debug.Log("Total player car count = " + totalPlayerCarCount);

        if (PlayerManager.LocalPlayerInstance == null)
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
            object playerPosition;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("position", out playerPosition);
            int index = (int) playerPosition;
            object color;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("color", out color);

            // if (color != null)
            // {
            //     localPlayerColor = color.ToString();
            //     
            //     //TODO Make Car color 
            //     
            //     testCar = PhotonNetwork.Instantiate(testMap[color.ToString()], positionMap[index].transform.position,
            //         positionMap[index].transform.rotation, 0);
            //     PhotonNetwork.LocalPlayer.TagObject = testCar;
            //     // Material colorMaterial = colorMap[color.ToString()];
            //     // testCar.transform.GetChild(0).GetComponent<MeshRenderer>().material = colorMaterial;
            // }

            object car;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("car", out car);

            string carName = car.ToString();
            string[] tokens = carName.Split('(');
            
            
            if (car != null)
            {
                testCar = PhotonNetwork.Instantiate(tokens[0], positionMap[index].transform.position,
                    positionMap[index].transform.rotation, 0);
                PhotonNetwork.LocalPlayer.TagObject = testCar;
            }

            if (testCar == null)
            {
                testCar = PhotonNetwork.Instantiate("Car1", positionMap[index].transform.position,
                    positionMap[index].transform.rotation, 0);
                PhotonNetwork.LocalPlayer.TagObject = testCar;
            }

            camera.GetComponent<PlayerCamera>().target = testCar.transform;
            // Destroy(defaultCamera);
            defaultCamera.gameObject.SetActive(false);

            PhotonNetwork.LocalPlayer.SetCustomProperties(
                new Hashtable() {{requestAdd, 1}, {"color", localPlayerColor}});

            PhotonNetwork.LocalPlayer.TagObject = testCar;
            Debug.Log("Tag objectd " + PhotonNetwork.LocalPlayer.TagObject);
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {{"indicator", color.ToString()}});


            var mode = PhotonNetwork.LocalPlayer.CustomProperties["mode"];
            var isMode = mode.ToString();

            if (isMode.Equals("Classic"))
            {
                testCar.GetComponent<MultiCar>().SetItemMode(false);
            }
            else
            {
                var itemManager = FindObjectOfType<ItemManager>();
                if (itemManager != null)
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


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var properties = PhotonNetwork.MasterClient.CustomProperties;

            Debug.Log("count " + totalPlayerCarCount);
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
            indicatorScript.color = colorMap[changedProps["indicator"].ToString()];
            foreach (MultiCar car in FindObjectsOfType<MultiCar>())
            {
                if (car.GetActorNumber() == targetPlayer.ActorNumber)
                {
                    indicatorScript.target = car.transform;
                    break;
                }
            }
        }

        CheckGameOver(changedProps);
    }

    private void CheckGameOver(Hashtable info)
    {
        if (mode.Equals("Classic"))
        {
            SoloMode(info);
        }
        else
        {
            Debug.Log("Team Mode in");
            TeamMode(info);
        }
    }

    private void TeamMode(Hashtable changedProps)
    {
        var color = changedProps["color"].ToString();
        if (!teamPlayerCount.ContainsKey(color))
        {
            teamPlayerCount.Add(color, 0);
        }

        ChangeTeamPlayerCount(changedProps, color, teamPlayerCount);
    }

    private void ChangeTeamPlayerCount(Hashtable changedProps, string color, Dictionary<string, int> teamCount)
    {
        if (changedProps.ContainsKey(requestAdd))
        {
            totalPlayerCarCount += (int) changedProps[requestAdd];
            teamCount[color] += (int) changedProps[requestAdd];
        }

        if (changedProps.ContainsKey(requestDelete))
        {
            teamCount[color] += (int) changedProps[requestDelete];
            CheckTeamGameOver(teamCount);
        }
    }

    private void CheckTeamGameOver(Dictionary<string, int> teamPlayer)
    {
        foreach (string teamColor in teamPlayer.Keys)
        {
            if (teamPlayer[teamColor] > 0)
            {
                if (IsTeamGameOver(teamPlayer, teamColor))
                {
                    _photonView.RPC("DisplayTeamWinner", RpcTarget.All, teamColor);
                }
            }
        }
    }

    private bool IsTeamGameOver(Dictionary<string, int> teamPlayer, string targetColor)
    {
        foreach (string teamPlayerKey in teamPlayer.Keys)
        {
            if (!teamPlayerKey.Equals(targetColor))
            {
                if (teamPlayer[teamPlayerKey] > 0)
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
        GameObject testObject = (GameObject) other.TagObject;
        if (testObject != null)
        {
            PhotonNetwork.Destroy(testObject);
        }

        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName);
    }


    public void LeaveRoom()
    {
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