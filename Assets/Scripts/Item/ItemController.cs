using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    private GameObject[] item;//ȹ���� ������ �߰� �迭

    public GameObject effect;
    public float respawnTime;
    private float spinspeed = 20.0f;
    Renderer renderer;
    public Canvas Canvas;

    public Image firstitem;
    public Image seconditem;


    public Image missile;
    public Image shield;
    public Image banana;


    // Start is called before the first frame update
    void Start()
    {
        //renderer = this.GetComponent<Renderer>();
        renderer = this.GetComponentInChildren<Renderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        float y = spinspeed * Time.deltaTime;
        transform.Rotate(Vector3.up * y);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag =="Player")
        {
            Instantiate(effect, this.transform);
            //Destroy�� ��ƼŬ�����տ� �����־���
            plusItem();
            StartCoroutine("FadeOut");

            //������ ȹ�� �ڵ�
            
            Invoke("respawn", respawnTime);
        }
    }

    void respawn()
    {
        StartCoroutine("FadeIn");
    }


    IEnumerator FadeOut()
    {
        int i = 10;
        while (i > 0)
        {
            i -= 1;
            float f = i / 10.0f;
            Color c = renderer.material.color;
            c.a = f;
            renderer.material.color = c;
            yield return new WaitForSeconds(0.02f);
        }
    }

    IEnumerator FadeIn()
    {
        int i = 0;
        while (i < 10)
        {
            i += 1;
            float f = i / 10.0f;
            Color c = renderer.material.color;
            c.a = f;
            renderer.material.color = c;
            yield return new WaitForSeconds(0.02f);
        }
    }

    private void plusItem()
    {
        int number = Random.Range(1, 4);

        if(number ==1)//�ٳ���
        {
            firstitem.sprite = banana.sprite;
        }
        else if(number == 2)//�̻���
        {
            firstitem.sprite = missile.sprite;
        }
        else // ����
        {
            firstitem.sprite = shield.sprite;
        }
    }
}
