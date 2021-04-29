using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Move(gameObject.transform, 0.5f));
    }


    IEnumerator Move(Transform tr, float y)
    {
        var i = 0f;
        var isZero = false;
        while (true)
        {
            if (isZero == false)
            {
                tr.Translate(0, 0.01f, 0);
                i += 0.01f;
                if (i >= y)
                {
                    isZero = true;
                }
                yield return new WaitForSeconds(0.05f);
            }
            else
            {
                tr.Translate(0, -0.01f, 0);
                i -= 0.01f;
                if (i <= 0)
                {
                    isZero = false;
                }
                yield return new WaitForSeconds(0.05f);
            }
        }
        yield return null;
    }

}
