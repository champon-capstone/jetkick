using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinnerItemInit : MonoBehaviour
{
    public Text winnerName;

    public void SetName(string name)
    {
        winnerName.text = name;
    }
}