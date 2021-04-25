using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using UnityEngine.UI;

public class Chat : MonoBehaviour, IChatClientListener
{
    private ChatClient chatClient;
    private string userName;
    private string lobbyChanel = "Lobby";
    private string currentChannel;
    
    public InputField textInputField;
    public Text outputText;

    private void Start()
    {
        Application.runInBackground = true;
        userName = PhotonNetwork.LocalPlayer.NickName;
        currentChannel = lobbyChanel;
        ConnectToChatServer();
    }

    private void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
    }

    #region Private Methods

    private void ConnectToChatServer()
    {
        Debug.Log("Request Connecting chat server");
        var settings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        chatClient = new ChatClient(this);
        chatClient.UseBackgroundWorkerForSending = true;
        chatClient.AuthValues = new AuthenticationValues(userName);
        chatClient.ConnectUsingSettings(settings);
    }

    #endregion
    
    #region Chat Callbacks
    
    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log(level+" "+message);
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected");
    }

    public void OnConnected()
    {
        Debug.Log("Connected to Chat Server");
        chatClient.Subscribe(new string[] {lobbyChanel});
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("ChatStateChanged "+state);
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (channelName.Equals(currentChannel))
        {
            //Update TextField
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        if (channelName.Equals(currentChannel))
        {
            //Update TextField
        }
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        foreach (var channel in channels)
        {
            var hi = userName + " is connected to chat channel : "+channel;
            chatClient.PublishMessage(channel, hi);
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        //TODO Display connected another channel
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        //Not used but implemented
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log("user "+user+" is subscribed to "+channel);
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log("user "+user+" is unsubscribed to "+channel);
    }
    
    #endregion
}
