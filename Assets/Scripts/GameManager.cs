using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Public Fields

    [Header("Car color")]
    public Material red;
    public Material green;
    public Material white;
    
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
    
    public static GameManager instance;
    
    #endregion

    private GameObject testCar;
    private string playerPrefab = "TestCar3";
    private Dictionary<int, GameObject> positionMap;
    private Dictionary<String, Material> colorMap;
    private Dictionary<string, string> testMap;
    
    #region Unity

    private void Awake()
    {
        colorMap = new Dictionary<string, Material>();
        colorMap.Add("GREEN", green);
        colorMap.Add("RED", red);
        colorMap.Add("WHITE", white);
        positionMap = new Dictionary<int, GameObject>();
        positionMap.Add(0, position1);
        positionMap.Add(1, position2);
        positionMap.Add(2, position3);
        positionMap.Add(3, position4);
        testMap = new Dictionary<string, string>();
        testMap.Add("WHITE", "TestCar3_white 1");
        testMap.Add("RED", "TestCar3_red 1");
        testMap.Add("GREEN", "TestCar3_green 1");

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

            Debug.Log("Color "+color);
            
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
        }
        else
        {
            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        }
        
        
    }
    
    #endregion
    
    
    #region Photon Callbacks
    
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
        PhotonNetwork.LeaveRoom();
    }
    
    #endregion
}
