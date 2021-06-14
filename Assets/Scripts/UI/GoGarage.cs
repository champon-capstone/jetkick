using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoGarage : MonoBehaviour
{
    public void GotoGarage()
    {
        SceneManager.LoadScene("Garage");
    }
}
