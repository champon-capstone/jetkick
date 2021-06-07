using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticField : MonoBehaviour
{
    public float speed = 100.0f;

    float delta = 0.01f;
    float interval = 0.01f;
    float time = 0.0f;
    public TimeCountdown timecountdown;

    // Start is called before the first frame update
    void Start()
    {
        interval = delta / speed;
        timecountdown = GameManager.instance.GetComponent<TimeCountdown>();
    }

    // Update is called once per frame
    void Update()
    {
        if (time > interval && timecountdown.EndTime())
        {
            Debug.Log("�ڱ��� ��ҵ��Գ�??");
            transform.localScale -= new Vector3(delta, delta, delta);
            time = 0.0f;
            // (ring out)player destroy everyone 

        }
        time += Time.deltaTime;
    }
}
