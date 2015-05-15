using UnityEngine;
using System.Collections;
using UnityEditor;

//this will make sure the static constructor being called
[InitializeOnLoad]
public static class AutoLoad
{
	//static constructor
	static AutoLoad()
	{
		EditorApplication.playmodeStateChanged += OnPlayModeChanged;
	}
	public static bool AutoLoadBoot {
		get { return EditorPrefs.GetBool ("BootAutoLoad",false); }
		set { EditorPrefs.SetBool ("BootAutoLoad", value); }
	}
	
	public static string MasterScene {
		get {
			string path = EditorPrefs.GetString ("MasterPath");
			return path;
		}
		set { EditorPrefs.SetString ("MasterPath", value); }
	}
	
	public static string PreviousScene {
		get { return EditorPrefs.GetString ("PreviousPath"); }
		set { EditorPrefs.SetString ("PreviousPath", value); }
	}
	
	public static void OnPlayModeChanged ()
	{
		//initialize uimanager setting screen
		UIManagerSettingEditor.bootSceneObj = Resources.LoadAssetAtPath (MasterScene,typeof(Object));
		if(MasterScene==""||MasterScene==null)
		{
			return;
		}

		if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
		{
			// User pressed play -- autoload master scene.
			PreviousScene = EditorApplication.currentScene;
			if (EditorApplication.SaveCurrentSceneIfUserWantsTo())
			{
				if (!EditorApplication.OpenScene(MasterScene))
				{
					Debug.LogError(string.Format("error: scene not found: {0}", MasterScene));
					EditorApplication.isPlaying = false;
				}
			}
			else
			{
				// User cancelled the save operation -- cancel play as well.
				EditorApplication.isPlaying = false;
			}
		}
		if (EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
		{
			// User pressed stop -- reload previous scene.
			if (!EditorApplication.OpenScene(PreviousScene))
			{
				Debug.LogError(string.Format("error: scene not found: {0}", PreviousScene));
			}
		}
	}
}
