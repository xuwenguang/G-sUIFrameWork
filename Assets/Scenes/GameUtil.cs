using UnityEngine;
using System.Collections;
using System;

public static class GameUtil
{
	static IEnumerator LoadNonUIScene(string sceneName,Action cb)
	{
		AsyncOperation async = Application.LoadLevelAdditiveAsync(sceneName);
		yield return async;
		if(cb!=null)
		{
			cb();
		}
	}
	
	public static void RemoveNonUIScene(string tag="NonUIObject")
	{
		GameObject interactivityObject = GameObject.FindGameObjectWithTag(tag);
		if (interactivityObject)
		{
			UnityEngine.Object.Destroy(interactivityObject);
			//free the memory
			Resources.UnloadUnusedAssets();
		}
	}
}
