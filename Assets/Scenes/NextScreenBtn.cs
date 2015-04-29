using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NextScreenBtn : MonoBehaviour {

	public string NextScreenName;
	public bool playAnim;
	void Awake()
	{
		GetComponent<Button>().onClick.AddListener(ShowNextScreen);
	}
	public void ShowNextScreen()
	{
		UIManager.Instance.ShowScreen(NextScreenName, playAnim);
	}

}
