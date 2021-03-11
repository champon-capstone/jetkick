using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// Basic car settings for CarControler
[System.Serializable]
public class CarSettings
{
    /// mass of the car rigidbody
    public float mass = 1500;


    /// drag of the car rigidbody
    public float drag = 0.05f;


    /// center of the mass of the car rigidbody
    public Vector3 centerOfMass = new Vector3(0, -1.0f, 0);
  

    /// motorTorque of the car (speed and acceleration)
    public float motorTorque = 1200;


    /// steering angle of the car
    public float steeringAngle = 50;
}
