using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;
    Transform pivot;
    public Transform camera;

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

    public float turnSpeed = 5.0f;

    struct View
    {
        public float distance;
        public float height;
    }

    static View makeView(float distance, float height)
    {
        View view;
        view.distance = distance;
        view.height = height;
        return view;
    }

    View[] views = new View[] {
        makeView(13.0f, 8.0f), makeView(20.0f, 15.0f), makeView(6.0f, 3.0f), makeView(-0.8f, 0.8f)
    };
    int viewIndex = 0;

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
            viewIndex++;
            viewIndex %= views.Length;

            distance = views[viewIndex].distance;
            height = views[viewIndex].height;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            automatic = !automatic;
            if (automatic)
            {
                camera.localPosition = Vector3.zero;
                transform.position = target.position + new Vector3(0, height, 0) + target.forward * -distance;
                camera.LookAt(target);
                pivot.localPosition = Vector3.zero;
                pivot.localRotation = Quaternion.identity;
            }
            else
            {
                camera.localPosition = new Vector3(0, 0, -(distance + 1.0f));
                camera.localRotation = Quaternion.identity;
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
        if (target == null)
        {
            return;
        }
        if (automatic)
        {
            if (distance > 0) // third person
            {
                transform.position = Vector3.SmoothDamp(transform.position, target.position + new Vector3(0, height, 0) + target.forward * -distance, ref velocity, smoothTime);
                camera.LookAt(target);
            }
            else // first person
            {
                transform.position = target.position + new Vector3(0, height, 0) + target.forward * -distance;
                transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * turnSpeed);
                camera.localRotation = Quaternion.identity;
            }
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
            camera.localPosition = Vector3.SmoothDamp(camera.localPosition, new Vector3(0, 0, -dist), ref velocity, smoothTime);
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
