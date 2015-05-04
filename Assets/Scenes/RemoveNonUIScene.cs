using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RemoveNonUIScene : MonoBehaviour {

	public string tag="NonUIObject";
	public bool autoTrigger=true;
	void Awake()
	{
		if(autoTrigger)
		{
			GetComponent<Button>().onClick.AddListener(RemoveGame);
		}
	}
	
	public void RemoveGame()
	{
		GameUtil.RemoveNonUIScene(tag);
	}
}
