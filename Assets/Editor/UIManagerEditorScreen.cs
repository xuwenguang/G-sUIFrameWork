using UnityEngine;
using System.Collections;
using UnityEditor;

public class UIManagerEditorScreen : EditorWindow {

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
	}
}
