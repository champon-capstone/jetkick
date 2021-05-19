using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConversationInit : MonoBehaviour
{
    public Text who;
    public Text message;

    public void initConversation(String whoText, String messageText)
    {
        who.text = whoText;
        message.text = messageText;
    }
    
}
