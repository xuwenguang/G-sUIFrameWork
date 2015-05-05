using UnityEngine;
using System.Collections;
using UnityEditor;

public class UIManagerEditorScreen : EditorWindow {

	//static variables can be persistent even you close the editor window and then open it, non static is not working like this
	public static bool autoLoad=false;
	public static Object bootSceneObj;

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

		//update build setting button in here


		//help link
//	http://docs.unity3d.com/ScriptReference/EditorGUILayout.ObjectField.html
	}
}
