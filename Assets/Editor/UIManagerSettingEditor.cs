using UnityEngine;
using System.Collections;
using UnityEditor;

public class UIManagerSettingEditor : EditorWindow {

	//static variables can be persistent even you close the editor window and then open it, non static is not working like this
	public static Object bootSceneObj;
	private bool showWarning=false;

	//force to focus back to this view if the setting is not right
	private bool forceFocus=false;

	// Add menu named "Setting" to the UIManager menu
	[MenuItem ("UIManager/Setting")] 
	static void Init () {
		EditorWindow.GetWindow(typeof(UIManagerSettingEditor));

		//initializing, or it wont show in the editor when you first time open the tab
		UIManagerSettingEditor.bootSceneObj = Resources.LoadAssetAtPath (AutoLoad.MasterScene,typeof(Object));
	}
	
	void OnGUI () 
	{
		GUILayout.Label ("UIManager Editor Settings",EditorStyles.boldLabel);
		EditorGUILayout.HelpBox ("Select the start scene in your game to auto load it when play in the editor ",MessageType.Info); 

		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("Start Scene");

		var previousBootScene=bootSceneObj;
		bootSceneObj = EditorGUILayout.ObjectField (bootSceneObj,typeof(Object),true);
		if(bootSceneObj!=previousBootScene)
		{
			//save the boot scene path in here
			AutoLoad.MasterScene=AssetDatabase.GetAssetPath(bootSceneObj);
		}

		EditorGUILayout.EndHorizontal ();
		if(GUILayout.Button("Clear Start Scene Data"))
		{
			//clear the editor prefs here
			System.IO.Directory.Delete(Application.persistentDataPath, true);
		}

		EditorGUILayout.HelpBox ("Create UI Element",MessageType.Info);
		

		if(GUILayout.Button("Generate Boot Screen Root"))
		{
			CreateObject.GenerateBootScreen();
		}

		if(GUILayout.Button("Generate Screen Root"))
		{
			CreateObject.CreateScreenRoot();
		}

		if(GUILayout.Button("Create Back Button"))
		{
			CreateObject.CreateBackButton();
		}

		if(GUILayout.Button("Create Next Button"))
		{
			CreateObject.CreateNextButton();
		}
		if(showWarning)
		{
			EditorGUILayout.HelpBox("Please select the start scene, or it will not be auto loaded when you play from Unity editor",MessageType.Warning);
		}
	}

	public void UpdateBuildSettingWhenPlayModeChange()
	{
		UIManager.Instance.UpdateBuildSettingScenes ();
	}

	void OnLostFocus()
	{
		//save master scene path and check if it is null
		string scenePath=AssetDatabase.GetAssetPath(bootSceneObj);
		if(bootSceneObj==null)
		{
			showWarning=true;

		}
		else if(!scenePath.Contains(".unity"))
		{
			EditorApplication.Beep();
			bootSceneObj=null;
			EditorUtility.DisplayDialog("The scene field should only contain a unity scene object","Please make sure everything inside the scene list are scene objects","OK");
		}
		else
		{
			showWarning=false;
		}
	}
}
