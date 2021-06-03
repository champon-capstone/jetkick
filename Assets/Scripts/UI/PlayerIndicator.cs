using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerIndicator : MonoBehaviour
{
    public Transform target;
    public Transform camera;

    public float height = 3.0f;
    Vector3 velocity = Vector3.zero;

    public string username;
    public Color color = new Color(0, 0, 0, 255);
    TMP_Text usernameText;

    // Start is called before the first frame update
    void Start()
    {
        usernameText = transform.GetChild(0).GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        usernameText.text = username;
        usernameText.color = color;
        try
        {
            transform.position = Vector3.SmoothDamp(transform.position, target.position + new Vector3(0, height, 0),
                ref velocity, 0);
        }
        catch (MissingReferenceException e)
        {
            Destroy(gameObject);
        }
        
        transform.LookAt(camera);
    }
}
