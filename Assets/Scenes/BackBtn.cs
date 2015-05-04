using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackBtn : MonoBehaviour {

	public bool PlayAnim;
	public bool autoTrigger=true;
	void Awake()
	{
		if(autoTrigger)
		{
			GetComponent<Button>().onClick.AddListener(OnBackBtnClicked);
		}
	}
	public void OnBackBtnClicked()
	{
		UIManager.Instance.BackButtonSelected(PlayAnim);
	}
}
