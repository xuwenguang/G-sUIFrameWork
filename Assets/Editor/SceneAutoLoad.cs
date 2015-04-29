using UnityEditor;
using UnityEngine;
/// <summary>
/// Scene auto loader.
/// </summary>
/// <description>
/// This class adds a File > Scene Autoload menu containing options to select
/// a "master scene" enable it to be auto-loaded when the user presses play
/// in the editor. When enabled, the selected scene will be loaded on play,
/// then the original scene will be reloaded on stop.
///
/// Based on an idea on this thread:
/// http://forum.unity3d.com/threads/157502-Executing-first-scene-in-build-settings-when-pressing-play-button-in-editor
/// </description>
[InitializeOnLoad]
static class SceneAutoLoader
{
    // Static constructor binds a playmode-changed callback.
    // [InitializeOnLoad] above makes sure this gets executed.
    static SceneAutoLoader()
    {
        EditorApplication.playmodeStateChanged += OnPlayModeChanged;
    }

    // Menu items to select the "master" scene and control whether or not to load it.
    [MenuItem("File/Scene Autoload/Select Master Scene...")]
    private static void SelectMasterScene()
    {
        string masterScene = EditorUtility.OpenFilePanel("Select Master Scene", Application.dataPath, "unity");
        if (!string.IsNullOrEmpty(masterScene))
        {
            int index = masterScene.IndexOf(Application.dataPath);
            string finalPath = (index < 0) ? masterScene : masterScene.Remove(index, Application.dataPath.Length);

            MasterScene = finalPath;
            LoadMasterOnPlay = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [MenuItem("File/Scene Autoload/Always Save Current Scene", true)]
    private static bool ShowAlwaysSaveCurrentScene()
    {
        return !AlwaysSaveCurrentScene;
    }
    [MenuItem("File/Scene Autoload/Always Save Current Scene")]
    private static void EnableAlwaysSaveCurrentScene()
    {
        AlwaysSaveCurrentScene = true;
    }

    [MenuItem("File/Scene Autoload/Don't Always Save Current Scene", true)]
    private static bool ShowDontAlwaysSaveCurrentScene()
    {
        return AlwaysSaveCurrentScene;
    }
    [MenuItem("File/Scene Autoload/Don't Always Save Current Scene")]
    private static void DisableAlwaysSaveCurrentScene()
    {
        AlwaysSaveCurrentScene = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [MenuItem("File/Scene Autoload/Load Master On Play", true)]
    private static bool ShowLoadMasterOnPlay()
    {
        return !LoadMasterOnPlay;
    }
    [MenuItem("File/Scene Autoload/Load Master On Play")]
    private static void EnableLoadMasterOnPlay()
    {
        LoadMasterOnPlay = true;
    }

    [MenuItem("File/Scene Autoload/Don't Load Master On Play", true)]
    private static bool ShowDontLoadMasterOnPlay()
    {
        return LoadMasterOnPlay;
    }
    [MenuItem("File/Scene Autoload/Don't Load Master On Play")]
    private static void DisableLoadMasterOnPlay()
    {
        LoadMasterOnPlay = false;
    }

    [MenuItem("Project Config/Save and Play &q")]
    private static void Play()
    {
        var state = AlwaysSaveCurrentScene;
        AlwaysSaveCurrentScene = true;
        EditorApplication.ExecuteMenuItem("Edit/Play");
        AlwaysSaveCurrentScene = state;
    }

	[MenuItem("Project Config/Clear Persistent Data")]
    private static void ClearPersistentData()
    {
        System.IO.Directory.Delete(Application.persistentDataPath, true);
    }

    // Play mode change callback handles the scene load/reload.
    private static void OnPlayModeChanged()
    {
        if (!LoadMasterOnPlay)
        {
            return;
        }

        if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
        {
            // User pressed play -- autoload master scene.
            PreviousScene = EditorApplication.currentScene;
            if (( AlwaysSaveCurrentScene && EditorApplication.SaveScene(PreviousScene) ) || EditorApplication.SaveCurrentSceneIfUserWantsTo())
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

    // Properties are remembered as editor preferences.
    private const string cEditorPrefLoadMasterOnPlay = "SceneAutoLoader.LoadMasterOnPlay";
    private const string cEditorPrefAlwaysSaveCurrentScene = "SceneAutoLoader.AlwaysSaveCurrentScene";
    private const string cEditorPrefMasterScene = "SceneAutoLoader.MasterScene";
    private const string cEditorPrefPreviousScene = "SceneAutoLoader.PreviousScene";

    private static bool LoadMasterOnPlay
    {
        get { return EditorPrefs.GetBool(cEditorPrefLoadMasterOnPlay, false); }
        set { EditorPrefs.SetBool(cEditorPrefLoadMasterOnPlay, value); }
    }
    private static bool AlwaysSaveCurrentScene
    {
        get { return EditorPrefs.GetBool(cEditorPrefAlwaysSaveCurrentScene, false); }
        set { EditorPrefs.SetBool(cEditorPrefAlwaysSaveCurrentScene, value); }
    }

    private static string MasterScene
    {
        get
        {
            string path = EditorPrefs.GetString(cEditorPrefMasterScene, "Master.unity");
			
			// Assuming windows-based volume structure (i.e. C:\)
            if (path.Contains(":"))
            {
                return path;
            }
            else
            {
                return Application.dataPath + path;
            }
        }
        set { EditorPrefs.SetString(cEditorPrefMasterScene, value); }
    }

    private static string PreviousScene
    {
        get { return EditorPrefs.GetString(cEditorPrefPreviousScene, EditorApplication.currentScene); }
        set { EditorPrefs.SetString(cEditorPrefPreviousScene, value); }
    }
}