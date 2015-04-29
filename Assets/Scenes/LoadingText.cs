using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class LoadingText : MonoBehaviour {
	public Text t;
	private bool flag=true;

	IEnumerator Start()
	{
		t=GetComponent<Text>();

		while(true)
		{
			if(flag)
			{
				flag=false;
				t.text="Loading";
			}
			else
			{
				t.text="Loading...";
				flag=true;
			}
			yield return new WaitForSeconds(0.3f);
		}

	}
}
