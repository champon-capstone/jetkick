using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public static GameManager instance;

    #endregion

    private GameObject testCar;
    private string playerPrefab = "TestCar3";
    private Dictionary<int, GameObject> positionMap;
    private Dictionary<string, Color> colorMap;
    private Dictionary<string, string> testMap;

    private string localPlayerColor;

    private string mode;

    private string requestCarCount = "playerCarCount";

    private int totalPlayerCarCount = 0;


    #region Unity

    private void Awake()
    {
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
            PhotonNetwork.MasterClient.SetCustomProperties(new Hashtable() {{requestCarCount, 0}, {"init", true}});

            object modeText;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("mode", out modeText);
            mode = (string) modeText;
        }
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

            if (color != null)
            {
                localPlayerColor = color.ToString();
                testCar = PhotonNetwork.Instantiate(testMap[color.ToString()], positionMap[index].transform.position,
                    positionMap[index].transform.rotation, 0);
                PhotonNetwork.LocalPlayer.TagObject = testCar;
                // Material colorMaterial = colorMap[color.ToString()];
                // testCar.transform.GetChild(0).GetComponent<MeshRenderer>().material = colorMaterial;
            }

            if (testCar == null)
            {
                testCar = PhotonNetwork.Instantiate("TestCar3_green 1", positionMap[index].transform.position,
                    positionMap[index].transform.rotation, 0);
                PhotonNetwork.LocalPlayer.TagObject = testCar;
            }

            camera.GetComponent<PlayerCamera>().target = testCar.transform;
            // Destroy(defaultCamera);
            defaultCamera.gameObject.SetActive(false);

            try
            {
                var count = (int) PhotonNetwork.MasterClient.CustomProperties[requestCarCount + " " + localPlayerColor];
                PhotonNetwork.MasterClient.SetCustomProperties(new Hashtable()
                    {{requestCarCount + " " + localPlayerColor, ++count}});
            }
            catch (NullReferenceException e)
            {
                StartCoroutine("RequestCarCountPlus");
            }

            PhotonNetwork.LocalPlayer.TagObject = testCar;
            Debug.Log("Tag objectd " + PhotonNetwork.LocalPlayer.TagObject);
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {{"indicator", color.ToString()}});
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

        if (targetPlayer.IsMasterClient)
        {
            if (changedProps.ContainsKey("init") && (bool) changedProps["init"] == true)
            {
                changedProps["init"] = false;
                return;
            }

            CheckGameOver(changedProps);
        }
    }

    private void CheckGameOver(Hashtable info)
    {
        if (mode.Equals("SPEED"))
        {
            SpeedMode(info);
        }
        else
        {
            ItemMode(info);
        }
    }

    private void ItemMode(Hashtable info)
    {
        if (!CheckIsContain(info))
        {
            return;
        }

        Dictionary<string, int> teamPlayerCount = new Dictionary<string, int>();

        foreach (string key in info.Keys)
        {
            if (key.Contains(requestCarCount))
            {
                var tokens = key.Split(' ');
                teamPlayerCount.Add(tokens[1], (int) info[key]);
            }
        }

        if (IsItemGameOver(teamPlayerCount))
        {
            Debug.Log("GameOver");
        }
    }

    private bool IsItemGameOver(Dictionary<string, int> teamPlayer)
    {
        var list = teamPlayer.Keys.ToArray();
        foreach (string key in teamPlayer.Keys)
        {
            if (teamPlayer[key] > 0)
            {
                foreach (string key1 in list)
                {
                    if (key != key1 && teamPlayer[key1] > 0)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private void SpeedMode(Hashtable info)
    {
        if (!CheckIsContain(info))
        {
            return;
        }

        totalPlayerCarCount = 0;
        foreach (string key in info.Keys)
        {
            if (key.Contains(requestCarCount))
            {
                totalPlayerCarCount += (int) info[key];
            }
        }

        if (totalPlayerCarCount <= 0)
        {
            Debug.Log("GameOver");
        }
    }

    private bool CheckIsContain(Hashtable info)
    {
        bool isCheck = false;
        foreach (string key in info.Keys)
        {
            if (key.Contains(requestCarCount))
            {
                isCheck = true;
                break;
            }
        }

        return isCheck;
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
        var count = (int) PhotonNetwork.MasterClient.CustomProperties[requestCarCount + " " + localPlayerColor];
        PhotonNetwork.MasterClient.SetCustomProperties(new Hashtable()
            {{requestCarCount + " " + localPlayerColor, --count}});
    }

    private IEnumerator RequestCarCountPlus()
    {
        yield return new WaitForSeconds(1f);
        // var count = (int) PhotonNetwork.MasterClient.CustomProperties[requestCarCount+localPlayerColor];
        PhotonNetwork.MasterClient.SetCustomProperties(new Hashtable() {{requestCarCount + " " + localPlayerColor, 1}});
    }
}