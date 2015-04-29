using UnityEngine;
using System.Collections;
using UnityEditor;

public static class AutoLoadBootScene
{

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
		Debug.LogWarning ("inside on playmode changed");
		if (!AutoLoadBoot){return;}
			
		if(EditorApplication.currentScene==MasterScene )
		{
			//if the user is going to stop playing 
			if(EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
				if (!EditorApplication.OpenScene (PreviousScene)) {
					Debug.LogError (string.Format ("error: scene not found: {0}", PreviousScene));
				}
			}
			else
			{
				//player is inside master scene, so do not need to do anything
				return;
			}
		}
		else
		{
			if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode) 
			{
				// User pressed play -- autoload master scene.
				PreviousScene = EditorApplication.currentScene;
				if ((EditorApplication.SaveScene (PreviousScene)) || EditorApplication.SaveCurrentSceneIfUserWantsTo ()) 
				{
					if (!EditorApplication.OpenScene (MasterScene)) 
					{
						Debug.LogError (string.Format ("error: scene not found: {0}", MasterScene));
						EditorApplication.isPlaying = false;
					}
				} 
				else 
				{
					// User cancelled the save operation -- cancel play as well.
					EditorApplication.isPlaying = false;
				}
			}
		}
	}
}
