using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackBtn : MonoBehaviour {

	public bool PlayAnimation;
	void Awake()
	{
		GetComponent<Button>().onClick.AddListener(OnBackBtnClicked);
	}
	public void OnBackBtnClicked()
	{
		UIManager.Instance.BackButtonSelected(PlayAnimation);
	}
}
