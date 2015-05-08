using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor (typeof(UIManager))]
public class UIManagerEditor : Editor 
{
	void OnEnable()
	{
		EditorApplication.playmodeStateChanged += CheckIfScreenIsNull;
	}


	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if(GUI.changed)
		{
			SceneTypeVerification();
//			UpdateMasterScenePath();
			UpdateBuildSetting();
		}
	}



	/// <summary>
	/// might have bug in here, need to debug
	/// to check if user drag wrong scene item into here
	/// </summary>
	public void SceneTypeVerification()
	{
		var um = target as UIManager;
		//check if the scene objects are unity scene
		for(int i=0;i<um.Screens.Length;i++)
		{
			var s=um.Screens[i].scene;
			if(s!=null)
			{
				string scenePath=AssetDatabase.GetAssetPath(s);
				if(!scenePath.Contains(".unity"))
				{
					EditorApplication.Beep();
					um.Screens[i].scene=null;
					EditorUtility.DisplayDialog("The scene field should only contain a unity scene object","Please make sure everything inside the scene list are scene objects","OK");
					Debug.LogError("the scene field should only contain a unity scene object");
				}

			}
		}
	}
//	public void UpdateMasterScenePath()
//	{
//		//get the reference of the uimanager script
//		var uiManager=target as UIManager;
//
//		if(uiManager.AutoLoadBootScene)
//		{
//			PlayerPrefs.SetString(UIManager.MasterScenePathKey,EditorApplication.currentScene);
//		}
//		else
//		{
//			PlayerPrefs.SetString(UIManager.MasterScenePathKey,"");
//		}
//		PlayerPrefs.Save();
//	}

	public void CheckIfScreenIsNull()
	{
		var um = target as UIManager;

		foreach(UIManager.Screen s in um.Screens)
		{
			if(s.scene==null)
			{
				Debug.Log(s.scene.name);
				EditorApplication.isPlaying=false;
				EditorApplication.Beep();
				EditorUtility.DisplayDialog("The scene in screens list should not be null","Put the scene object in scene slot or remove the element from the list","OK");
				Debug.LogError("The scene in screens list should not be null");
				return;
			}
		}
	}

	public void UpdateBuildSetting()
	{
		var uimanager = target as UIManager;
		uimanager.UpdateBuildSettingScenes ();
	}
}
