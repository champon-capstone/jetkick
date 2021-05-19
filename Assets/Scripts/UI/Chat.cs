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
    private float yValue = 0;
    
    public GameObject content;
    public GameObject conversation;
    
    public InputField chatInputField;
    private Text outputText;

    #region Unity

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

    #endregion
   

    #region Public Methods

    public void OnSendMessage()
    {
        var message = chatInputField.text;
        chatInputField.text = "";
        chatClient.PublishMessage(currentChannel, message);
    }

    #endregion
    
    #region Private Methods

    private void ConnectToChatServer()
    {
        var settings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        chatClient = new ChatClient(this);
        chatClient.UseBackgroundWorkerForSending = true;
        chatClient.AuthValues = new AuthenticationValues(userName);
        chatClient.ConnectUsingSettings(settings);
    }

    private void AddMessage(string who,string message)
    {
        Debug.Log(message);
        // outputText.text += "  "+message + "\r\n";
        var conversationObject = Instantiate(conversation, new Vector3(0, yValue, 0), Quaternion.identity);
        conversationObject.transform.SetParent(content.transform);
        conversationObject.GetComponent<ConversationInit>().initConversation(who, message);
        yValue -= conversation.GetComponent<RectTransform>().rect.y;
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
        chatClient.Subscribe(new string[] {lobbyChanel});
    }

    public void ConnectToRoomChat(string chatRoom)
    {
        chatClient.Unsubscribe(new string[]{lobbyChanel});
        chatClient.Subscribe(new string[] {chatRoom});
        outputText.text = "";
        currentChannel = chatRoom;
    }

    public void ConnectToLobby()
    {
        string[] channels = new string[chatClient.PublicChannels.Count];
        Debug.Log("Channel count "+chatClient.PublicChannels.Count);
        var index = 0;
        foreach (var channel in chatClient.PublicChannels.Values)
        {
            channels[index++] = channel.Name;
        }

        currentChannel = lobbyChanel;
        chatClient.Unsubscribe(channels);
        chatClient.Subscribe(new string[] {lobbyChanel});
        outputText.text = "";
    }
    
    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("ChatStateChanged "+state);
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (channelName.Equals(currentChannel))
        {
            AddMessage(senders[0], messages[0]+"");
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
