using UnityEngine;
using System.Collections;

public class AnimationTest : MonoBehaviour {

	private Animator anim;
	public AnimationClip inAnim;
	void Awake()
	{
//		inAnim.legacy=true;
		anim=this.GetComponent<Animator>();
		Destroy(anim);
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
