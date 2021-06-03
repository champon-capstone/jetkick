using System;
using UnityEngine;
using UnityEngine.UI;

public class ConversationInit : MonoBehaviour
{
    public Text message;

    public void initConversation(String whoText, String messageText)
    {
        var msg = whoText + " : " + messageText;
        message.text = msg;
    }
}