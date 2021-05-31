using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Object = System.Object;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Public Fields

    [Header("Position")] 
    public GameObject position1;
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

    private string requestCarCount = "playerCarCount";
    
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
        testMap.Add("WHITE", "TestCar3_white 1");
        testMap.Add("RED", "TestCar3_red 1");
        testMap.Add("GREEN", "TestCar3_green 1");

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            PhotonNetwork.MasterClient.SetCustomProperties(new Hashtable() {{requestCarCount, 0}, {"init", true}});    
        }
    }
    private void Start()
    {
        instance = this;

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
                testCar = PhotonNetwork.Instantiate(testMap[color.ToString()], positionMap[index].transform.position, Quaternion.identity, 0);
                PhotonNetwork.LocalPlayer.TagObject = testCar;
                // Material colorMaterial = colorMap[color.ToString()];
                // testCar.transform.GetChild(0).GetComponent<MeshRenderer>().material = colorMaterial;
            }

            if (testCar == null)
            {
                testCar = PhotonNetwork.Instantiate("TestCar3_green 1", positionMap[index].transform.position, Quaternion.identity, 0);
                PhotonNetwork.LocalPlayer.TagObject = testCar;
            }

            camera.GetComponent<PlayerCamera>().target = testCar.transform;
            // Destroy(defaultCamera);
            defaultCamera.gameObject.SetActive(false);

            try
            {
                var count = (int) PhotonNetwork.MasterClient.CustomProperties[requestCarCount];
                PhotonNetwork.MasterClient.SetCustomProperties(new Hashtable() {{requestCarCount, ++count}});
            }
            catch (NullReferenceException e)
            {
                StartCoroutine("RequestCarCountPlus");
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {{"indicator", color.ToString()}});
        }
        else
        {
            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        }
    }
    
    #endregion
    
    
    #region Photon Callbacks

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
            if (changedProps.ContainsKey("init") && (bool)changedProps["init"] == true)
            {
                changedProps["init"] = false;
                return;
            }
            if (changedProps.ContainsKey(requestCarCount))
            {
                if ((int)changedProps[requestCarCount] <= 0)
                {
                    Debug.Log("GameOver");
                }
            }
        }
    }

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName); // not seen if you're the player connecting
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        var testObject = (GameObject) other.TagObject;
        Destroy(testObject);
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
    }

    #endregion

   

    #region Public Methods

    public void LeaveRoom()
    {
        if (PlayerManager.LocalPlayerInstance != null)
        {
            var count = (int) PhotonNetwork.MasterClient.CustomProperties[requestCarCount];
            PhotonNetwork.MasterClient.SetCustomProperties(new Hashtable() {{requestCarCount, --count}});
        }
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    private IEnumerator RequestCarCountPlus()
    {
        yield return new WaitForSeconds(2f);
        var count = (int) PhotonNetwork.MasterClient.CustomProperties[requestCarCount];
        PhotonNetwork.MasterClient.SetCustomProperties(new Hashtable() {{requestCarCount, ++count}});
    }
    
}
