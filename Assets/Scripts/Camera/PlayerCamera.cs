using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;
    Transform pivot;
    Transform camera;

    public float distance = 13.0f;
    public float height = 8.0f;
    public float smoothTime = 0.3f;
    Vector3 velocity = Vector3.zero;

    public bool automatic = true;

    public float followSpeed = 3;
    public float mouseSpeed = 2;

    float turnSmoothing = .1f;
    public float minAngle = -35;
    public float maxAngle = 35;

    float smoothX;
    float smoothY;
    float smoothXvelocity;
    float smoothYvelocity;
    public float lookAngle;
    public float tiltAngle;

    struct Preset
    {
        public float distance;
        public float height;
    }

    static Preset makePreset(float distance, float height)
    {
        Preset preset;
        preset.distance = distance;
        preset.height = height;
        return preset;
    }

    Preset[] presets = new Preset[] {
        makePreset(13.0f, 8.0f), makePreset(20.0f, 15.0f), makePreset(6.0f, 3.0f)
    };
    int presetIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        pivot = transform.GetChild(0);
        camera = pivot.GetChild(0);

        if (automatic)
        {
            transform.position = target.position + new Vector3(0, height, 0) + target.forward * -distance;
            camera.LookAt(target);
        }
        else
        {
            pivot.position = target.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            presetIndex++;
            presetIndex %= presets.Length;

            distance = presets[presetIndex].distance;
            height = presets[presetIndex].height;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            automatic = !automatic;
            if (automatic)
            {
                camera.localPosition = new Vector3(0, 0, 0);
                transform.position = target.position + new Vector3(0, height, 0) + target.forward * -distance;
                camera.LookAt(target);
            }
            else
            {
                camera.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }
    }

    void FixedUpdate()
    {
        if (!automatic)
        {
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");

            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
            HandleRotations(Time.deltaTime, v, h, mouseSpeed);
        }
    }

    void LateUpdate()
    {
        if (automatic)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target.position + new Vector3(0, height, 0) + target.forward * -distance, ref velocity, smoothTime);
            camera.LookAt(target);
        }
        else
        {
            pivot.position = target.position;

            float dist = distance + 1.0f;
            Ray ray = new Ray(camera.parent.position, camera.position - camera.parent.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, dist))
            {
                if (hit.transform.tag == "Wall")
                {
                    dist = hit.distance - 0.25f;
                }
            }

            if (dist > distance)
            {
                dist = distance;
            }
            camera.localPosition = new Vector3(0, 0, -dist);
        }
    }

    void HandleRotations(float d, float v, float h, float targetSpeed)
    {
        if (automatic)
        {
            return;
        }

        if (turnSmoothing > 0)
        {
            smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXvelocity, turnSmoothing);
            smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYvelocity, turnSmoothing);
        }
        else
        {
            smoothX = h;
            smoothY = v;
        }

        tiltAngle -= smoothY * targetSpeed;
        tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
        pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);

        lookAngle += smoothX * targetSpeed;
        transform.rotation = Quaternion.Euler(0, lookAngle, 0);
    }
}
