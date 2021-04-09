using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;
    Transform camera;

    public float distance = 13.0f;
    public float height = 8.0f;
    public float smoothTime = 0.3f;
    Vector3 velocity = Vector3.zero;

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
        camera = transform;
        camera.position = target.position + new Vector3(0, height, 0) + target.forward * -distance;
        camera.LookAt(target);
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
        camera.position = Vector3.SmoothDamp(transform.position, target.position + new Vector3(0, height, 0) + target.forward * -distance, ref velocity, smoothTime);
        camera.LookAt(target);
    }
}
