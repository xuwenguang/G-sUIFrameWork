using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HidePopUpBtn : MonoBehaviour {

	public string PopUpScreenName;
	public bool PlayTransitionAnimation=true;
	public bool autoTrigger=true;
	void Awake()
	{
		if(autoTrigger)
		{
			GetComponent<Button>().onClick.AddListener(HidePopUp);
		}
	}
	
	public void HidePopUp()
	{
		UIManager.Instance.HidePopUp(PopUpScreenName,PlayTransitionAnimation);
	}
}
