using UnityEngine;
using System.Collections;

public class StartScreenScript : MonoBehaviour {

	public void PreTransitionIn()
	{
		GameUtil.RemoveNonUIScene();
	}
}
