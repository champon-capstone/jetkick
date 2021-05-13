using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice;
using Photon.Voice.PUN;

public class VoiceController : MonoBehaviour
{
    private PhotonVoiceNetwork _network;

    private void Awake()
    {
        this._network = PhotonVoiceNetwork.Instance;
        _network.AutoConnectAndJoin = true;
    }
    
    
}
