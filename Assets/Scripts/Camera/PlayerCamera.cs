using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;

    public float distance = 13.0f;
    public float height = 8.0f;
    public float smoothTime = 0.3f;
    Vector3 velocity = Vector3.zero;

    struct Preset
    {
        public float distance;
        public float height;
    }

    Preset[] presets = new Preset[3];
    int presetIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        presets[0].distance = 13.0f;
        presets[0].height = 8.0f;
        presets[1].distance = 20.0f;
        presets[1].height = 15.0f;
        presets[2].distance = 6.0f;
        presets[2].height = 3.0f;

        transform.position = target.position + new Vector3(0, height, 0) + target.forward * -distance;
        transform.LookAt(target);
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
    }

    void FixedUpdate()
    {

    }

    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position + new Vector3(0, height, 0) + target.forward * -distance, ref velocity, smoothTime);
        transform.LookAt(target);
    }
}
