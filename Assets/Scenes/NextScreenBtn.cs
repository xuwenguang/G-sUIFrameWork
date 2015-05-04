using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NextScreenBtn : MonoBehaviour {

	public string NextScreenName;
	public bool playAnim;
	public bool autoTrigger=true;
	void Awake()
	{
		if(autoTrigger)
		{
			GetComponent<Button>().onClick.AddListener(ShowNextScreen);
		}
	}
	public void ShowNextScreen()
	{
		UIManager.Instance.ShowScreen(NextScreenName, playAnim);
	}

}
