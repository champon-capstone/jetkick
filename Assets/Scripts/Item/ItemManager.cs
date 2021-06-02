using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemManager : MonoBehaviour
{
    public Image firstitem;
    public Image seconditem;

    public Image empty;
    public Image missile;
    public Image shield;
    public Image banana;
    private bool firstitemEmpty = true;
    private bool seconditemEmpty = true;
    // Start is called before the first frame update
    void Start()
    {
        //init itemEmpty
        firstitem.sprite = empty.sprite;
        seconditem.sprite = empty.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            if(!firstitemEmpty)
            {
                //using item and delete UI
                if(firstitem.sprite == missile.sprite)//Firstitem == missile
                {
                    Debug.Log("using missile");
                }
                else if(firstitem.sprite == shield.sprite)//Firstitem == shield
                {
                    Debug.Log("using shield");
                }
                else if(firstitem.sprite == banana.sprite) //Firstitem == banana
                {
                    Debug.Log("using banana");
                }
              

                //change image
                if(!seconditemEmpty)
                {
                    firstitem.sprite = seconditem.sprite;
                    seconditem.sprite = empty.sprite;
                    seconditemEmpty = true;
                }
                else //seconditemEmpty
                {
                    firstitem.sprite = empty.sprite;
                }
            }
            else//no item 
            {
                Debug.Log("Empty Item");
            }
        }
    }

    public void plusItem()
    {
        
        int number = Random.Range(1, 4);//random 1~3
        if (firstitemEmpty)//first item empty
        {
            Debug.Log("get Firstitem");
            if (number == 1)//banana
            {
                firstitem.sprite = banana.sprite;
            }
            else if (number == 2)//missile
            {
                firstitem.sprite = missile.sprite;
            }
            else //shield
            {
                firstitem.sprite = shield.sprite;
            }
            firstitemEmpty = false;
        }
        else if (seconditemEmpty)//second item empty
        {
            Debug.Log("get seconditem");
            if (number == 1)//banana
            {
                seconditem.sprite = banana.sprite;
            }
            else if (number == 2)//missile
            {
                seconditem.sprite = missile.sprite;
            }
            else //shield
            {
                seconditem.sprite = shield.sprite;
            }
            seconditemEmpty = false;
        }
        else // full item 
        {
            Debug.Log("item full");
        }
    }
}
