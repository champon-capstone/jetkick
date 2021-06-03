using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallBlock : MonoBehaviour
{
    
	public bool warning = false;
	private float number = 0.0f;
	Renderer renderer;

    private void Start()
    {
		renderer = this.GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.tag == "Player")
		{
			FallWarning();
		}
	}

    public void FallWarning()
	{
		if(number == 2.0f)
        {
			
			//Debug.Log("Destroy 2 second");
			Destroy(gameObject, 3.0f);
		}
		if (warning)
		{
			StartCoroutine(FadeOut());
			//Debug.Log("warning fadeout");
			
			warning = false;
			number++;
			
		}
		else
        {
			StartCoroutine(FadeIn());
			//Debug.Log("warning fadein");
			warning = true;
			number++;
		}
		
	}

	IEnumerator FadeOut()
	{
		//Debug.Log("fadeout");
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
		warning = false;
		FallWarning();
	}

	IEnumerator FadeIn()
	{
		//Debug.Log("fadein");
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
		warning = true;
		FallWarning();
	}
}
