using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CreateObject  
{
	[MenuItem("GameObject/UI/Screen Root")]
	public static void CreateScreenRoot()
	{
		string[] path = EditorApplication.currentScene.Split (char.Parse("/"));
		string sceneName ="Screen_"+ path [path.Length - 1].Replace(".unity","");
		GameObject screenRoot = new GameObject (sceneName);
		screenRoot.AddComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;
		screenRoot.AddComponent<CanvasScaler>();
		screenRoot.AddComponent<GraphicRaycaster>();
		screenRoot.AddComponent<ScreenRoot>();

		var eventSys = new GameObject ("EventSystem");
		eventSys.AddComponent<EventSystem>();
		eventSys.AddComponent<StandaloneInputModule>();
		eventSys.AddComponent<TouchInputModule>();

		GameObject camera;
		if(Camera.allCamerasCount==0)
		{
			camera=new GameObject("MainCamera");
			camera.AddComponent<Camera>();
		}
		else
		{
			camera=Camera.allCameras[0].gameObject;
		}

		camera.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
		//set up the depth of the camera so it can overlay on top of other none ui screens
		camera.GetComponent<Camera>().depth = -10;

		camera.transform.SetParent(screenRoot.transform,true);
		eventSys.transform.SetParent (screenRoot.transform,true);
	}

	[MenuItem("GameObject/UI/Next Screen Button")]
	public static void CreateNextButton()
	{
		var btn = new GameObject ("NextScreenButton");
		btn.AddComponent<Button>();
		btn.AddComponent<Image>();
		btn.AddComponent<NextScreenBtn>().playAnim=true;
		GameObject text = new GameObject ("Text");
		Text t = text.AddComponent<Text> ();
		t.text="next";
		t.alignment = TextAnchor.MiddleCenter;
		t.color = Color.black;
		btn.GetComponent<RectTransform>().sizeDelta=new Vector2(80f,30f);

		text.transform.SetParent (btn.transform,true);

		//make sure the button is under the selected hierarchy and been selected after creation
		btn.transform.SetParent (Selection.activeTransform,false);
		Selection.activeObject = btn;
	}

	[MenuItem("GameObject/UI/Back Button")]
	public static void CreateBackButton()
	{
		var btn = new GameObject ("BackButton");
		btn.AddComponent<Button>();
		btn.AddComponent<Image>();
		btn.AddComponent<BackBtn>().PlayAnim=true;
		GameObject text = new GameObject ("Text");
		Text t = text.AddComponent<Text> ();
		t.text="back";
		t.alignment = TextAnchor.MiddleCenter;
		t.color = Color.black;
		btn.GetComponent<RectTransform>().sizeDelta=new Vector2(80f,30f);
		
		text.transform.SetParent (btn.transform,true);
		btn.transform.SetParent (Selection.activeTransform,false);
		Selection.activeObject = btn;
	}

	[MenuItem("GameObject/UI/Generate Boot Screen")]
	public static void GenerateBootScreen()
	{
		GameObject boot = new GameObject ("Boot");
		boot.AddComponent<UIManager>();

		Camera cam = new GameObject ("Camera").AddComponent<Camera>();
		cam.clearFlags = CameraClearFlags.Depth;
		//set up the depth of the camera so it can overlay on top of other none ui screens
		cam.depth = -10;

		cam.transform.SetParent (boot.transform,true);

		var eventSys = new GameObject ("EventSystem");
		eventSys.AddComponent<EventSystem>();
		eventSys.AddComponent<StandaloneInputModule>();
		eventSys.AddComponent<TouchInputModule>();

		eventSys.transform.SetParent (boot.transform,true);

		GameObject bootCanvas = new GameObject ("Boot_Canvas");
		bootCanvas.AddComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;
		bootCanvas.AddComponent<CanvasScaler> ();
		bootCanvas.AddComponent<GraphicRaycaster>();

		GameObject loadingText = new GameObject ("LoadingText");
		Text t = loadingText.AddComponent<Text> ();
		t.alignment=TextAnchor.MiddleLeft;
		t.text="Loading...";
		loadingText.AddComponent<LoadingText>();
		loadingText.transform.SetParent (bootCanvas.transform,false);
	}


	
}
