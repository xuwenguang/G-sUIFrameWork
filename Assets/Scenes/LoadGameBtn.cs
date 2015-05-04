using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadGameBtn : MonoBehaviour {

	public string sceneName;
	public UnityEvent callBack;
	public bool autoTrigger=true;
	void Awake()
	{
		if(autoTrigger)
		{
			GetComponent<Button>().onClick.AddListener(LoadGame);
		}
	}

	public void LoadGame()
	{
		StartCoroutine(GameUtil.LoadNonUIScene(sceneName,()=>{callBack.Invoke();}));
	}
}
