using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ItemGenerator
{

    public void MissileAttack(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        var missile = PhotonNetwork.Instantiate("missile", spawnPosition, spawnRotation);
    }

    public void SpawnBanana(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        var banana = PhotonNetwork.Instantiate("missile", spawnPosition, spawnRotation);
    }

    public void ActiveShield(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        var shield = PhotonNetwork.Instantiate("missile", spawnPosition, spawnRotation);
    }
}