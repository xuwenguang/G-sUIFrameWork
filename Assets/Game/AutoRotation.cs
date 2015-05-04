using UnityEngine;
using System.Collections;

public class AutoRotation : MonoBehaviour {


	public float speed=1f;
	void Update()
	{
		transform.Rotate(0,speed * Time.deltaTime,0);
	}
}
