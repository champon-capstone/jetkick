using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallBlock : MonoBehaviour
{
    public float fallTime = 2.5f;
	public bool warning = false;
	private float number = 0.0f;
	Renderer renderer;
    // Start is called before the first frame update
	void OnCollisionEnter(Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts)
		{
			renderer = gameObject.GetComponent<Renderer>();
			if (collision.gameObject.tag == "Player")
			{
				StartCoroutine(Fall(fallTime));
			}
		}
	}

	IEnumerator Fall(float time)
	{
		if(number == 4.0f)
        {
			Debug.Log("Destroy");
			Destroy(gameObject);
		}
		if (warning)
		{
			StartCoroutine(FadeOut());
			Debug.Log("warningt");
			yield return new WaitForSeconds(time);
			warning = false;
			number++;
			StartCoroutine(Fall(fallTime));
		}
		else
        {
			StartCoroutine(FadeIn());
			Debug.Log("warningf");
			yield return new WaitForSeconds(time);
			warning = true;
			number++;
			StartCoroutine(Fall(fallTime));
		}
		
	}

	IEnumerator FadeOut()
	{
		Debug.Log("fadeout");
		float i = 100.0f;
		while (i > 0)
		{
			i -= 0.1f;
			float f = i / 10.0f;
			Color c = renderer.material.color;
			c.a = f;
			renderer.material.color = c;
			yield return new WaitForSeconds(0.2f);
		}
	}

	IEnumerator FadeIn()
	{
		Debug.Log("fadein");
		float i = 0f;
		while (i < 100)
		{
			i += 0.1f;
			float f = i / 10.0f;
			Color c = renderer.material.color;
			c.a = f;
			renderer.material.color = c;
			yield return new WaitForSeconds(0.2f);
		}
	}
}
