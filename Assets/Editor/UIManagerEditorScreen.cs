using UnityEngine;
using System.Collections;
using UnityEditor;

public class UIManagerEditorScreen : EditorWindow {

	public static bool autoLoad=false;
	public Object bootSceneObj;

	// Add menu named "Setting" to the UIManager menu
	[MenuItem ("UIManager/Setting")] 
	static void Init () {
		EditorWindow.GetWindow(typeof(UIManagerEditorScreen));
	}
	
	void OnGUI () 
	{
		//create autoload checkbox here
		//maybe add all the screen list to here
		//reference the boot scene in here

		GUILayout.Label ("UIManager Editor Settings",EditorStyles.boldLabel);
		EditorGUILayout.HelpBox ("Drag the Boot scene of your game into the object field and if the Auto-load toggle is checked, every time you play the game it will start from your boot scene",MessageType.Info); 
		autoLoad = EditorGUILayout.Toggle ("Auto-load boot scene",autoLoad);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("Boot Scene");
		bootSceneObj = EditorGUILayout.ObjectField (bootSceneObj,typeof(Object),true);
		EditorGUILayout.EndHorizontal ();

		//help link
	http://docs.unity3d.com/ScriptReference/EditorGUILayout.ObjectField.html
	}
}
