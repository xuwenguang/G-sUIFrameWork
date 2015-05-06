using UnityEngine;
using System.Collections;
using UnityEditor;

public class UIManagerEditorScreen : EditorWindow {

	//static variables can be persistent even you close the editor window and then open it, non static is not working like this
	public static bool autoLoad=false;
	public static Object bootSceneObj;
	private bool showWarning=false;

	// Add menu named "Setting" to the UIManager menu
	[MenuItem ("UIManager/Setting")] 
	static void Init () {
		EditorWindow.GetWindow(typeof(UIManagerEditorScreen));
	}
	
	void OnGUI () 
	{
		GUILayout.Label ("UIManager Editor Settings",EditorStyles.boldLabel);
		EditorGUILayout.HelpBox ("check here to auto load boot scene ",MessageType.Info); 
		autoLoad = EditorGUILayout.Toggle ("Auto-load boot scene",autoLoad);
		if(autoLoad)
		{
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Boot Scene");
			bootSceneObj = EditorGUILayout.ObjectField (bootSceneObj,typeof(Object),true);
			EditorGUILayout.EndHorizontal ();
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
			EditorGUILayout.HelpBox("Please select the boot scene, or there will be errors when play the game",MessageType.Error);
		}
	}

	public void UpdateBuildSettingWhenPlayModeChange()
	{
		UIManager.Instance.UpdateBuildSettingScenes ();
	}

	void OnLostFocus()
	{
		//save master scene path and check if it is null
		if(autoLoad==true && bootSceneObj==null)
		{
			EditorApplication.Beep();
			showWarning=true;
			EditorUtility.DisplayDialog("Please Select the Boot Scene","You have to select the boot scenein the UIManager setting window or uncheck the auto-load boot scene option","OK");
			this.Focus();//???
		}
		else
		{
			showWarning=false;
		}
	}
}
