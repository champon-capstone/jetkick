using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.UI;


public class VehicleControl : MonoBehaviour
{
    public bool activeControl = true;

    private PhotonView _photonView;
    
    [System.Serializable]
    public class ConnectWheel
    {
        public bool frontWheelDrive = true;
        public Transform frontLeftWheel;
        public Transform frontRightWheel;

        public bool backWheelDrive = true;
        public Transform backLeftWheel;
        public Transform backRightWheel;

    }

    public CarWheels carWheels;

    [System.Serializable]
    public class CarWheels
    {
        public ConnectWheel wheels;
        public WheelSetting setting;
    }


    [System.Serializable]
    public class WheelSetting
    {
        public float Rad = 0.5f;
        public float wheelmass = 800.0f;
        public float Distance = 0.2f;
    }

    public CarSounds carSounds;

    [System.Serializable]
    public class CarSounds
    {
        public AudioSource normalEngine, LowEngine, HighEngine;

        public AudioSource nitro;
        public AudioSource switchGear;
    }

    public CarParticles carParticles;

    [System.Serializable]
    public class CarParticles
    {
        public GameObject brakeParticle;
        public ParticleSystem shiftParticle1, shiftParticle2;
        private GameObject[] wheelParticle = new GameObject[4];
    }

 

    public CarSetting carSetting;

    [System.Serializable]
    public class CarSetting
    {
      
        public Transform carSteer;
        public HitGround[] hitGround;

        public List<Transform> cameraSwitchView;

        public float Spring = 14000.0f;
        public float Damper = 1500.0f;  

        public float Power = 200f;
        public float shiftPower = 100f;
        public float brake_Power = 50000f;

        public Vector3 Centre = new Vector3(0.0f, -0.8f, 0.0f);

        public float maximumSteerAngle = 50.0f;

        public float shiftDownRPM = 3000.0f;    
        public float shiftUpRPM = 6000.0f;
        public float normalRPM = 450.0f;

        public float stiffness = 2.0f;

        public bool autoGear = true;

        public float[] gears = { -10f, 9f, 6f, 4.5f, 3f, 2.5f };


        public float LimitBackSpeed = 150.0f;
        public float LimitFrontSpeed = 250.0f;

    }

    


    [System.Serializable]
    public class HitGround
    {
        public string tag = "street";
        public bool grounded = false;
        public AudioClip brakeSound;
        public Color brakeColor;
    }

    private float steer = 0;
    private float accelation = 0.0f;
    [HideInInspector]
    public bool brake;

    private bool shiftmotor;

    [HideInInspector]
    public float currentTorque = 100f;
    [HideInInspector]
    public float power_Shift = 100;
    [HideInInspector]
    public bool shift;

    private float torque = 100f;

    [HideInInspector]
    public float speed = 0.0f;

    private float last_speed = -10.0f;


    private bool shifting = false;


    float[] efficienct_Table = { 0.6f, 0.65f, 0.7f, 0.75f, 0.8f, 0.85f, 0.9f, 1.0f, 1.0f, 0.95f, 0.80f, 0.70f, 0.60f, 0.5f, 0.45f, 0.40f, 0.36f, 0.33f, 0.30f, 0.20f, 0.10f, 0.05f };


    float efficienct_TableStep = 250.0f;

    private float Pitch;
    private float Delay_Pitch;

    private float shiftTime = 0.0f;

    private float DelayShift = 0.0f;


    [HideInInspector]
    public int curGear = 0;
    [HideInInspector]
    public bool NtralGear = true;

    [HideInInspector]
    public float motor_RPM = 0.0f;

    [HideInInspector]
    public bool Backmove = false;



    private float wantedRPM = 0.0f;
    private float w_rotate;
    private float slip1, slip2 = 0.0f;

    private GameObject[] Particle = new GameObject[4];

    private Vector3 steerCurrentAngle;

    private Rigidbody carRigidbody;

    private WheelComponent[] wheels;

    private class WheelComponent
    {
        public Transform wheel;
        public WheelCollider collider;
        public Vector3 startPosition;
        public float rotation1 = 0.0f;
        public float rotation2 = 0.0f;
        public float maximumSteer;
        public bool drive;
        public float position_y = 0.0f;
    }


    private WheelComponent SetWheelComponent(Transform wheel, float maximumSteer, bool drive, float position_y)
    {
        WheelComponent result = new WheelComponent();
        GameObject wheelCol = new GameObject(wheel.name + "WheelCollider");

        wheelCol.transform.parent = transform;
        wheelCol.transform.position = wheel.position;
        wheelCol.transform.eulerAngles = transform.eulerAngles;
        position_y = wheelCol.transform.localPosition.y;

        WheelCollider col = (WheelCollider)wheelCol.AddComponent(typeof(WheelCollider));

        result.wheel = wheel;
        result.collider = wheelCol.GetComponent<WheelCollider>();
        result.drive = drive;
        result.position_y = position_y;
        result.maximumSteer = maximumSteer;
        result.startPosition = wheelCol.transform.localPosition;

        return result;

    }

    void Awake()
    {
        _photonView = PhotonView.Get(this);
        if (carSetting.autoGear) NtralGear = false;

        carRigidbody = transform.GetComponent<Rigidbody>();

        wheels = new WheelComponent[4];

        wheels[0] = SetWheelComponent(carWheels.wheels.frontLeftWheel, carSetting.maximumSteerAngle, carWheels.wheels.frontWheelDrive, carWheels.wheels.frontLeftWheel.position.y);
        wheels[1] = SetWheelComponent(carWheels.wheels.frontRightWheel, carSetting.maximumSteerAngle, carWheels.wheels.frontWheelDrive, carWheels.wheels.frontRightWheel.position.y);

        wheels[2] = SetWheelComponent(carWheels.wheels.backLeftWheel, 0, carWheels.wheels.backWheelDrive, carWheels.wheels.backLeftWheel.position.y);
        wheels[3] = SetWheelComponent(carWheels.wheels.backRightWheel, 0, carWheels.wheels.backWheelDrive, carWheels.wheels.backRightWheel.position.y);
       

        if (carSetting.carSteer)
        steerCurrentAngle = carSetting.carSteer.localEulerAngles;

        foreach (WheelComponent w in wheels)
        {
            WheelCollider col = w.collider;
            col.suspensionDistance = carWheels.setting.Distance;
            JointSpring js = col.suspensionSpring;

            js.spring = carSetting.Spring;
            js.damper = carSetting.Damper;
            col.suspensionSpring = js;


            col.radius = carWheels.setting.Rad;

            col.mass = carWheels.setting.wheelmass;
           
        }
    }

    public void ShiftUp()
    {
        float now = Time.timeSinceLevelLoad;

        if (now < DelayShift) return;

        if (curGear < carSetting.gears.Length - 1)
        {
            carSounds.switchGear.GetComponent<AudioSource>().Play();
            if (!carSetting.autoGear)
            {
                if (curGear == 0)
                {
                    if (NtralGear){curGear++;NtralGear = false;}
                    else
                    { NtralGear = true;}
                }
            else
            {
                curGear++;
            }
        }
        else
        {
            curGear++;
        }

           DelayShift = now + 1.0f;
           shiftTime = 1.5f;
        }
    }

    public void ShiftDown()
    {
        float now = Time.timeSinceLevelLoad;

       if (now < DelayShift) return;

        if (curGear > 0 || NtralGear)
        {
            carSounds.switchGear.GetComponent<AudioSource>().Play();
            if (!carSetting.autoGear)
            {
                if (curGear == 1)
                {
                    if (!NtralGear){curGear--;NtralGear = true;}
                }
                else if (curGear == 0){NtralGear = false;}else{curGear--;}
            }
            else
            {
                curGear--;
            }

            DelayShift = now + 0.1f;
            shiftTime = 2.0f;
        }
    }



    void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.root.GetComponent<VehicleControl>())
        {
            collision.transform.root.GetComponent<VehicleControl>().slip2 = Mathf.Clamp(collision.relativeVelocity.magnitude, 0.0f, 10.0f);

            carRigidbody.angularVelocity = new Vector3(-carRigidbody.angularVelocity.x * 0.5f, carRigidbody.angularVelocity.y * 0.5f, -carRigidbody.angularVelocity.z * 0.5f);
            carRigidbody.velocity = new Vector3(carRigidbody.velocity.x, carRigidbody.velocity.y * 0.5f, carRigidbody.velocity.z);
        }

    }



    void OnCollisionStay(Collision collision)
    {

       if (collision.transform.root.GetComponent<VehicleControl>())
            collision.transform.root.GetComponent<VehicleControl>().slip2 = 5.0f;

    }



    void FixedUpdate()
    {
        if (_photonView == null)
        {
            return;
        }

        if (!_photonView.IsMine)
        {
            return;
        }
        speed = carRigidbody.velocity.magnitude * 2.7f;

        if (speed < last_speed - 10 && slip1 < 10)
        {
            slip1 = last_speed / 15;
        }
        last_speed = speed;

        if (slip2 != 0.0f)
            slip2 = Mathf.MoveTowards(slip2, 0.0f, 0.1f);

        carRigidbody.centerOfMass = carSetting.Centre;


        if (activeControl)
        {
                accelation = 0;
                brake = false;
                shift = false;

                if (carWheels.wheels.frontWheelDrive || carWheels.wheels.backWheelDrive)
                {
                    steer = Mathf.MoveTowards(steer, Input.GetAxis("Horizontal"), 0.2f);
                    accelation = Input.GetAxis("Vertical");
                    brake = Input.GetButton("Jump");
                    shift = Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift);
                }
        }
        

        if (!carWheels.wheels.frontWheelDrive && !carWheels.wheels.backWheelDrive)
            accelation = 0.0f;

        if (carSetting.carSteer)
            carSetting.carSteer.localEulerAngles = new Vector3(steerCurrentAngle.x, steerCurrentAngle.y, steerCurrentAngle.z + (steer * -120.0f));

        if (carSetting.autoGear && (curGear == 1) && (accelation < 0.0f))
        {
            if (speed < 5.0f)
                ShiftDown();
        }
        else if (carSetting.autoGear && (curGear == 0) && (accelation > 0.0f))
        {
            if (speed < 5.0f)
                ShiftUp();
        }
        else if (carSetting.autoGear && (motor_RPM > carSetting.shiftUpRPM) && (accelation > 0.0f) && speed > 10.0f && !brake)
        {
            ShiftUp();
        }
        else if (carSetting.autoGear && (motor_RPM < carSetting.shiftDownRPM) && (curGear > 1))
        {
            ShiftDown();
        }

        if (speed < 1.0f) Backmove = true;

        if (curGear == 0 && Backmove == true)
        {
            if (speed < carSetting.gears[0] * -10)
                accelation = -accelation;
        }
        else
        {
            Backmove = false;
        }



        wantedRPM = (5600.0f * accelation) * 0.1f + wantedRPM * 0.85f;

        float rpm = 0.0f;
        int motorizedWheels = 0;
        bool floorContact = false;
        int currentWheel = 0;

        foreach (WheelComponent w in wheels)
        {
            WheelHit hit;
            WheelCollider col = w.collider;

            if (w.drive)
            {
                if (!NtralGear && brake && curGear < 2)
                {
                    rpm += accelation * carSetting.normalRPM;
                }
                else
                {
                    if (!NtralGear)
                    {
                        rpm += col.rpm;
                    }else{
                        rpm += (carSetting.normalRPM*accelation);
                    }
                }


                motorizedWheels++;
            }

            if (brake || accelation < 0.0f)
            {
                if ((accelation < 0.0f) || (brake && (w == wheels[2] || w == wheels[3])))
                {
                    if (brake && (accelation > 0.0f))
                    {
                        slip1 = Mathf.Lerp(slip1, 5.0f, accelation * 0.01f);
                    }
                    else if (speed > 1.0f)
                    {
                        slip1 = Mathf.Lerp(slip1, 1.0f, 0.002f);
                    }
                    else
                    {
                        slip1 = Mathf.Lerp(slip1, 1.0f, 0.02f);
                    }
                    wantedRPM = 0.0f;
                    col.brakeTorque = carSetting.brake_Power;
                    w.rotation1 = w_rotate;
                }
            }
            else
            {
                col.brakeTorque = accelation == 0 || NtralGear ? col.brakeTorque = 1000 : col.brakeTorque = 0;

                slip1 = speed > 0.0f ?
    (speed > 100 ? slip1 = Mathf.Lerp(slip1, 1.0f + Mathf.Abs(steer), 0.02f) : slip1 = Mathf.Lerp(slip1, 1.5f, 0.02f))
    : slip1 = Mathf.Lerp(slip1, 0.01f, 0.02f);


                w_rotate = w.rotation1;

            }

            WheelFrictionCurve fc = col.forwardFriction;

            fc.asymptoteValue = 5000.0f;
            fc.extremumSlip = 2.0f;
            fc.asymptoteSlip = 20.0f;
            fc.stiffness = carSetting.stiffness / (slip1 + slip2);
            col.forwardFriction = fc;
            fc = col.sidewaysFriction;
            fc.stiffness = carSetting.stiffness / (slip1 + slip2);

            fc.extremumSlip = 0.2f + Mathf.Abs(steer);

            col.sidewaysFriction = fc;

            if (shift && (curGear > 1 && speed > 50.0f) && shiftmotor && Mathf.Abs(steer) < 0.2f)
            {

                if (power_Shift == 0) { shiftmotor = false; }

                power_Shift = Mathf.MoveTowards(power_Shift, 0.0f, Time.deltaTime * 10.0f);

                carSounds.nitro.volume = Mathf.Lerp(carSounds.nitro.volume, 0.5f, Time.deltaTime * 10.0f);

                if (!carSounds.nitro.isPlaying)
                {
                    carSounds.nitro.GetComponent<AudioSource>().Play();

                }
                currentTorque = power_Shift > 0 ? carSetting.shiftPower : carSetting.Power;
                carParticles.shiftParticle1.emissionRate = Mathf.Lerp(carParticles.shiftParticle1.emissionRate, power_Shift > 0 ? 50 : 0, Time.deltaTime * 10.0f);
                carParticles.shiftParticle2.emissionRate = Mathf.Lerp(carParticles.shiftParticle2.emissionRate, power_Shift > 0 ? 50 : 0, Time.deltaTime * 10.0f);
            }
            else
            {
                if (power_Shift > 20)
                {
                    shiftmotor = true;
                }

                carSounds.nitro.volume = Mathf.MoveTowards(carSounds.nitro.volume, 0.0f, Time.deltaTime * 2.0f);

                if (carSounds.nitro.volume == 0)
                    carSounds.nitro.Stop();

                power_Shift = Mathf.MoveTowards(power_Shift, 100.0f, Time.deltaTime * 5.0f);
                currentTorque = carSetting.Power;
                carParticles.shiftParticle1.emissionRate = Mathf.Lerp(carParticles.shiftParticle1.emissionRate, 0, Time.deltaTime * 10.0f);
                carParticles.shiftParticle2.emissionRate = Mathf.Lerp(carParticles.shiftParticle2.emissionRate, 0, Time.deltaTime * 10.0f);
            }

            w.rotation1 = Mathf.Repeat(w.rotation1 + Time.deltaTime * col.rpm * 360.0f / 60.0f, 360.0f);
            w.rotation2 = Mathf.Lerp(w.rotation2,col.steerAngle,0.1f);
            w.wheel.localRotation = Quaternion.Euler(w.rotation1,w.rotation2, 0.0f);

            Vector3 lp = w.wheel.localPosition;

            if (col.GetGroundHit(out hit))
            {
                if (carParticles.brakeParticle)
                {
                    if (Particle[currentWheel] == null)
                    {
                        Particle[currentWheel] = Instantiate(carParticles.brakeParticle, w.wheel.position, Quaternion.identity) as GameObject;
                        Particle[currentWheel].name = "WheelParticle";
                        Particle[currentWheel].transform.parent = transform;
                        Particle[currentWheel].AddComponent<AudioSource>();
                        Particle[currentWheel].GetComponent<AudioSource>().maxDistance = 30;
                        Particle[currentWheel].GetComponent<AudioSource>().spatialBlend = 1;
                        Particle[currentWheel].GetComponent<AudioSource>().dopplerLevel = 5;
                        Particle[currentWheel].GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Custom;
                    }

                    var pc = Particle[currentWheel].GetComponent<ParticleSystem>();
                    bool WGrounded = false;

                    for (int i = 0; i < carSetting.hitGround.Length; i++)
                    {
                        if (hit.collider.CompareTag(carSetting.hitGround[i].tag))
                        {
                            WGrounded = carSetting.hitGround[i].grounded;

                            if ((brake || Mathf.Abs(hit.sidewaysSlip) > 0.5f) && speed > 1)
                            {
                                Particle[currentWheel].GetComponent<AudioSource>().clip = carSetting.hitGround[i].brakeSound;
                            }
                            
                        }
                    }


                    if (WGrounded && speed > 5 && !brake)
                    {
                        pc.enableEmission = true;
                        Particle[currentWheel].GetComponent<AudioSource>().volume = 0.5f;

                        if (!Particle[currentWheel].GetComponent<AudioSource>().isPlaying)
                            Particle[currentWheel].GetComponent<AudioSource>().Play();

                    }
                    else if ((brake || Mathf.Abs(hit.sidewaysSlip) > 0.6f) && speed > 1)
                    {
                        if ((accelation < 0.0f) || ((brake || Mathf.Abs(hit.sidewaysSlip) > 0.6f) && (w == wheels[2] || w == wheels[3])))
                        {
                            if (!Particle[currentWheel].GetComponent<AudioSource>().isPlaying)
                                Particle[currentWheel].GetComponent<AudioSource>().Play();
                            pc.enableEmission = true;
                            Particle[currentWheel].GetComponent<AudioSource>().volume = 3;
                        }
                    }
                    else
                    {
                        pc.enableEmission = false;
                        Particle[currentWheel].GetComponent<AudioSource>().volume = Mathf.Lerp(Particle[currentWheel].GetComponent<AudioSource>().volume, 0, Time.deltaTime * 10.0f);
                    }

                }


                lp.y -= Vector3.Dot(w.wheel.position - hit.point, transform.TransformDirection(0, 1, 0) / transform.lossyScale.x) - (col.radius);
                lp.y = Mathf.Clamp(lp.y, -10.0f, w.position_y);
                floorContact = floorContact || (w.drive);


            }
            else
            {
                if (Particle[currentWheel] != null)
                {
                    var pc = Particle[currentWheel].GetComponent<ParticleSystem>();
                    pc.enableEmission = false;
                }

                lp.y = w.startPosition.y - carWheels.setting.Distance;
                carRigidbody.AddForce(Vector3.down * 5000);
            }

            currentWheel++;
            w.wheel.localPosition = lp;
        }

        if (motorizedWheels > 1)
        {
            rpm = rpm / motorizedWheels;
        }


        motor_RPM = 0.95f * motor_RPM + 0.05f * Mathf.Abs(rpm * carSetting.gears[curGear]);
        if (motor_RPM > 5500.0f) motor_RPM = 5200.0f;

        int index = (int)(motor_RPM / efficienct_TableStep);
        if (index >= efficienct_Table.Length) index = efficienct_Table.Length - 1;
        if (index < 0) index = 0;

        float newTorque = currentTorque * carSetting.gears[curGear] * efficienct_Table[index];

        foreach (WheelComponent w in wheels)
        {
            WheelCollider col = w.collider;

            if (w.drive)
            {

                if (Mathf.Abs(col.rpm) > Mathf.Abs(wantedRPM))
                {

                    col.motorTorque = 0;
                }
                else
                {

                    float currentTorqueCol = col.motorTorque;

                    if (!brake && accelation != 0 && NtralGear == false)
                    {
                        if ((speed < carSetting.LimitFrontSpeed && curGear > 0) ||
                            (speed < carSetting.LimitBackSpeed && curGear == 0))
                        {

                            col.motorTorque = currentTorqueCol * 0.9f + newTorque * 1.0f;
                        }
                        else
                        {
                            col.motorTorque = 0;
                            col.brakeTorque = 2400;
                        }
                    }
                    else
                    {
                        col.motorTorque = 0;
                    }
                }
            }


            if (brake || slip2 > 2.0f)
            {
                col.steerAngle = Mathf.Lerp(col.steerAngle, steer * w.maximumSteer, 0.02f);
            }
            else
            {
                float SteerAngle = Mathf.Clamp(speed / carSetting.maximumSteerAngle, 1.0f, carSetting.maximumSteerAngle);
                col.steerAngle = steer * (w.maximumSteer / SteerAngle);
            }

        }



        Pitch = Mathf.Clamp(1.2f + ((motor_RPM - carSetting.normalRPM) / (carSetting.shiftUpRPM - carSetting.normalRPM)), 1.0f, 10.0f);

        shiftTime = Mathf.MoveTowards(shiftTime, 0.0f, 0.1f);


        if (Pitch == 1)
        {
            carSounds.normalEngine.volume = Mathf.Lerp(carSounds.normalEngine.volume, 1.0f, 0.1f);
            carSounds.LowEngine.volume = Mathf.Lerp(carSounds.LowEngine.volume, 0.5f, 0.1f);
            carSounds.HighEngine.volume = Mathf.Lerp(carSounds.HighEngine.volume, 0.0f, 0.1f);
        }
        else
        {

            carSounds.normalEngine.volume = Mathf.Lerp(carSounds.normalEngine.volume, 1.0f - Pitch, 0.1f);

            if ((Pitch > Delay_Pitch || accelation > 0) && shiftTime == 0.0f)
            {
                carSounds.LowEngine.volume = Mathf.Lerp(carSounds.LowEngine.volume, 0.0f, 0.2f);
                carSounds.HighEngine.volume = Mathf.Lerp(carSounds.HighEngine.volume, 1.0f, 0.1f);
            }
            else
            {
                carSounds.LowEngine.volume = Mathf.Lerp(carSounds.LowEngine.volume, 0.5f, 0.2f);
                carSounds.HighEngine.volume = Mathf.Lerp(carSounds.HighEngine.volume, 0.0f, 0.2f);
            }

            carSounds.HighEngine.pitch = Pitch;
            carSounds.LowEngine.pitch = Pitch;

            Delay_Pitch = Pitch;
        }

    }


    
}