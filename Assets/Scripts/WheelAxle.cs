using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Settings of visuals and physics of a left and right wheel.
[System.Serializable]
public class WheelAxle
{
    // Left WheelCollider
    public WheelCollider wheelColliderLeft;

    // Right WheelCollider
    public WheelCollider wheelColliderRight;

    //Left Wheel Mesh
    public GameObject wheelMeshLeft;

    // Right Wheel Mesh
    public GameObject wheelMeshRight;

    // Is motor torque applyed to this axle
    public bool motor;

    // Is this is a stearing axle
    public bool steering;
}

