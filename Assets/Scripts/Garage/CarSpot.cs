using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpot : MonoBehaviour
{
    [SerializeField]
    private Light light;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spot(0f, 85f));
    }

    private IEnumerator Spot(float a, float b)
    {
        //Debug.Log("spot µé¾î¿È");
        float timer = 0f;
        while (light.spotAngle <= 85f)
        {
            timer += Time.unscaledDeltaTime;
            light.spotAngle = Mathf.Lerp(a, b, timer);
            yield return null;
        }
    }

}
