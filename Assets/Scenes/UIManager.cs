﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// this code makes sure each of the scenes in the list can be automically added into build setting
/// need to add master scene into build setting manully though
/// </summary>
public class UIManager : MonoBehaviour {

	public const string MasterScenePathKey="masterscenepath";
	private static UIManager instance;
	public static UIManager Instance
	{
		get
		{
			return instance;
		}
	}

	public bool AutoLoadUIScenes=true;

#if UNITY_EDITOR
	public bool AutoUpdateBuildSetting=true;
#endif

	[Serializable]
	public class Screen {
		public bool ActiveOnLoad;
		public bool CantTransitionBack;
		public UnityEngine.Object scene;
		public string Name
		{
			get
			{
				if(scene!=null)
				{
					return scene.name.Replace(".unity","");
				}
				else
					return null;
			}
		}
	}
	
	public Screen[] Screens;

	private Dictionary <string,Screen> ScreenDict=new Dictionary<string, Screen>();

//	 Use this for initialization
	void Awake()
	{
		if(instance==null)
		{
			instance=this;
		}
		else
		{
			Debug.LogError("there is two uimanager script active at the same time");
		}

		//initialize screenDict, it is more efficient this way, if do a parse every time when call the show screen function, it will be O(n)
		foreach(Screen s in Screens)
		{
			ScreenDict.Add(s.scene.name,s);
		}


		EventSystemComp=GameObject.Find("Boot/EventSystem").GetComponent<EventSystem>();
		EventSystemComp.enabled = false;
		BootCanvasGO = GameObject.Find ("Boot_Canvas");
		if(BootCanvasGO==null)
		{
			Debug.LogError("boot canvas is nul");
		}
		UICamera = GameObject.Find ("Boot/Camera").GetComponent<Camera>();
	}
#if UNITY_EDITOR
	public void UpdateBuildSettingScenes()
	{
		if(AutoUpdateBuildSetting)
		{
			List<EditorBuildSettingsScene> sceneList = new List<EditorBuildSettingsScene> ();
			
			//update master scene build setting
			string bootScenePath = EditorPrefs.GetString ("MasterPath");
			sceneList.Add (new EditorBuildSettingsScene(bootScenePath,true));
			
			foreach(EditorBuildSettingsScene s in EditorBuildSettings.scenes)
			{
				sceneList.Add(s);
			}
			
			foreach(Screen screen in Screens)
			{
				bool inList=false;
				string scenePath=AssetDatabase.GetAssetPath(screen.scene);
				EditorBuildSettings.scenes.ToList().ForEach(x=>{
					if(x.path==scenePath)
					{
						inList=true;
					}
				});
				
				if(!inList)
					sceneList.Add(new EditorBuildSettingsScene(scenePath,true));
			}
			EditorBuildSettings.scenes = sceneList.ToArray ();
		}

	}
#endif

	IEnumerator Start()
	{
		bool isLoading=true;
		Setup(
			()=>{
			isLoading=false;
		});
		while(isLoading)
		{
			yield return null;
		}
		ShowInitialScreen();
	}

	//screen list, saving the uiscreen script and screen,string should be the scene name
	private Dictionary<Screen, ScreenRoot> _screenDict;

	
	//[note] commented all the places that use canvasRoot, because want to add more flexibility to the ui system that all the screens can use different camera settings, also avoided the Scrollrect flickering problem
	//if problem occurs, will add this back
	//	public Canvas CanvasRoot;
	private bool _isLoading;
	public bool IsLoadingInProgress { get { return _isLoading; } private set { _isLoading = value; } }

	[HideInInspector]
	public EventSystem EventSystemComp;
	[HideInInspector]
	public GameObject BootCanvasGO;
	[HideInInspector]
	public Camera UICamera;
	private Screen _currentScreen=new Screen();
	private Screen _previousScreen=new Screen();
		
	public System.Action OnDisableBackButton;
	public System.Action OnEnableBackButton;
	
	//to store all the previous screens (for back btn)
	private List<Screen> previousScreenList = new List<Screen>();
	
	public ScreenRoot GetScreenScript(Screen screen)
	{
		return _screenDict [screen];
	}

	public void Setup(System.Action setupCB=null)
	{
		StartCoroutine(SetupCoro(setupCB));
	}

	//load all the screens
	public IEnumerator SetupCoro(System.Action setupCB)
	{
		_screenDict=new Dictionary<Screen,ScreenRoot>();
		_isLoading = true;

		foreach ( Screen s in Screens )
		{
			string sceneName=s.scene.name;
			yield return Application.LoadLevelAdditiveAsync( sceneName );

			GameObject sceneGameObject = GameObject.Find( "Screen_" + sceneName );
			DebugUtil.Assert( sceneGameObject != null, "can not find screen : " + sceneName );

			ScreenRoot sceneScript = sceneGameObject.GetComponent<ScreenRoot>();
#if UNITY_EDITOR
			if(_screenDict.ContainsKey(s))
			{
				EditorApplication.Beep();
				EditorApplication.isPlaying=false;
				Debug.LogError("should not have duplicated scenes in screen list");
			}
#endif
			_screenDict.Add( s, sceneScript );
		}

		_isLoading = false;
		
		if (setupCB != null)
		{
			setupCB ();
		}
	}
	
	public void ShowInitialScreen()
	{
		foreach(Screen s in Screens)
		{
			if(s.ActiveOnLoad==true)
			{
				ShowScreen(s.scene.name);
			}
		}

		//Destory boot canvas
		//
		UnityEngine.Object.Destroy( BootCanvasGO );
	}
	
	public void ToggleGlobalInput( bool inputEnabled )
	{
		EventSystemComp.enabled=inputEnabled;
	}

	public void ShowScreen(string screenName, bool playAnimation=true, System.Action setupCB = null )
	{
		Screen screen=GetScreen(screenName);
		ScreenRoot screenScript=GetScreenScript(screen);
	
		PushPreviousScreenToPreviousScreenList(screen);
		
		StartCoroutine( _TransitionScreenIn( screen, playAnimation, () =>
		                                                     {
			if ( setupCB != null )
			{
				setupCB();
			}
		} ) );
		
	}

	public Screen GetScreen(string screenName)
	{
		try
		{
			return ScreenDict[screenName];
		}
		catch(KeyNotFoundException e)
		{
			Debug.LogError("there is no scene called "+screenName);
#if UNITY_EDITOR
			EditorApplication.isPlaying=false;
#endif
			return new Screen();
		}
	}

	
	IEnumerator _TransitionScreenIn( Screen newScene, bool playAnim, System.Action setupCB ,bool isBackBtn=false)
	{
		
		ToggleGlobalInput( false );
		
		ScreenRoot screenIn = _screenDict[newScene];
		ScreenRoot screenOut = _screenDict.ContainsKey( _currentScreen ) ? _screenDict[_currentScreen] : null;
		
		// Overlay or going from boot -> splash, show immediatly
		//
		if( IsOverlay( newScene ) || _currentScreen.scene == null)
		{
			//overlay popup screen do not need to be transition back to so no need to have back transition animations
			yield return StartCoroutine( _PerformTransition( screenIn, setupCB ) );
		}
		else // Perform full transition between current and new
		{       
			yield return StartCoroutine( _PerformTransition( screenIn, screenOut,playAnim, setupCB,isBackBtn ) );
		}
		
		
		screenIn.SetSceneActiveState(true);
		screenIn.OnSceneBecameVisible();
		
		// Update scene vars if not transitioning into overlay
		//
		if ( !IsOverlay( newScene ) )
		{         
			_previousScreen = _currentScreen;
			_currentScreen = newScene;
						
		}
		
		
		yield return StartCoroutine( screenIn._TransitionIn( playAnim ,isBackBtn) );
		screenIn.OnTransitionInComplete();
	
		ToggleGlobalInput(true);
	}
	
	//to see the screen is a UIOverlay screen or not (uioverlay screen is like pop up menus)
	private bool IsOverlay( Screen scene )
	{
		if ( !_screenDict.ContainsKey( scene ) )
		{
			return false;
		}
		return _screenDict[scene].IsPopUp;
	}
	
	IEnumerator _PerformTransition( ScreenRoot screenIn, ScreenRoot screenOut,bool playAnimation, System.Action setupCB ,bool isBackBtn=false)
	{
		screenOut.OnPreTransitionOut();
		screenIn.OnPreTransitionIn();
		
		// Callback which allows screen coming in to start its transition in 
		// animation early
		//
		bool startTransitionIn = false;
		System.Action fn = () =>
		{
			startTransitionIn = true;
		};
		screenOut.StartTransitionnInEvent += fn;
		
		
		if ( setupCB != null )
		{
			setupCB();
		}
		
		// Transition out current screen
		//
		StartCoroutine( screenOut._TransitionOut(  () =>
		                                                          {
			startTransitionIn = true;
			
			screenOut.OnTransitionOutComplete();
			screenOut.SetSceneActiveState( false );
			screenOut.OnSceneBecameInvisible();
			
			screenIn.OnPreviousScreenTransitionedOut();
		}
		, playAnimation,isBackBtn
		) );
		
		// Wait until transition finishes or we manually trigger out
		while ( !startTransitionIn )
		{
			yield return null;
		}
		
		screenOut.StartTransitionnInEvent -= fn;
		
		yield return StartCoroutine( SetupScreenIn( setupCB, screenIn ) );
	}
	
	
	IEnumerator _PerformTransition( ScreenRoot screenIn, System.Action setupCB )
	{
		if ( setupCB != null )
		{
			setupCB();
		}
		
		screenIn.OnPreTransitionIn();
		yield return StartCoroutine( SetupScreenIn( setupCB, screenIn ) );
		screenIn.OnTransitionInComplete();
	}
	
	IEnumerator SetupScreenIn( System.Action setupCB, ScreenRoot screenIn )
	{
		if ( screenIn == null )
		{
			yield break;
		}
		
		//if ( setupCB != null )
		//{
		//    setupCB();
		//}
		yield return StartCoroutine( screenIn.Initialize() );
	}


	/// the root of popUp screen should be another script inherite from UIRoot
	/// function use to show all the popup screens
	public void ShowPopUp()
	{
		
	}
	//use to hide overlay screens, like top menu and message box
	public void HidePopUp(string screenName, bool playAnimation ) 
	{
		Screen screen=GetScreen(screenName);
		if(GetScreenScript(screen).IsPopUp)
		{
			StartCoroutine( _OverlayTransitionOut( screen, null ,playAnimation) );
		}
	}
	
	//transition out function only for overlay screens	
	IEnumerator _OverlayTransitionOut( Screen currentScene, System.Action setupCB,bool playAnim=true,bool isBackBtn=false)
	{
		ScreenRoot screenOut = _screenDict[currentScene];
		ScreenRoot screenIn = null;
		
		screenOut.OnPreTransitionOut();
		
		// Transition out current screen
		// do not put all the screen active state function in the transition out function, this can separate the functionality of the code, will be easier to maintance in the future
		yield return StartCoroutine( screenOut._TransitionOut(() =>
		                                                      {
			screenOut.OnTransitionOutComplete();
			screenOut.SetSceneActiveState( false );
			screenOut.OnSceneBecameInvisible();
			
		},playAnim,isBackBtn));
		
		
		screenOut.OnTransitionOutComplete();
		
		yield return StartCoroutine( SetupScreenIn( setupCB, screenIn ) );
	}
	
	
	#region back button
	
	/// <summary>
	/// should do all the functions in the screen which have the back button
	/// use event & delegate
	/// update visiability and etc.
	/// </summary>
	public void DisableBackButton()
	{
		if ( OnDisableBackButton != null ) OnDisableBackButton();
		//clear previous screen list every time show or hide back button
		ClearPreviousScreenList ();
	}
	
	public void EnableBackButton()
	{
		if ( OnEnableBackButton != null ) OnEnableBackButton();
	}
	
	
	//should set the default parameter as the ui screen's default transition back animation string
	public void GoToPreviousScreen(bool playAnim)
	{
		string animTransitionIn="";
		string animTransitionOut="";
		//work on the move back animation
		//default animation is null, or we can setup a default animation name
		int lastIndex = previousScreenList.Count - 1;
		
		try
		{
			Screen lastScreenInList = previousScreenList[lastIndex];
			if(lastScreenInList.scene!=_currentScreen.scene)
			{
				//clear previous animation state
				ResetScreenAnimation(lastScreenInList);
				ResetScreenAnimation(_currentScreen);
		
				//do not use show screen is because do not want to add this screen to the previous screen list
				if(!lastScreenInList.CantTransitionBack)
				{
					//call the function on UIScreen script, the move background function is in there
					StartCoroutine( _TransitionScreenIn( lastScreenInList, playAnim,
					                                                     ()=>{_screenDict[_currentScreen].BackButtonSelected();},true ) );
					previousScreenList.RemoveAt (lastIndex);
				}
				//remove the last one in the list after press back button once
			}
			
		}
		catch (ArgumentOutOfRangeException e)
		{
			Debug.LogWarning("nothing in the previousScreenList, can not go back");
		}
	}
	
	public void ResetScreenAnimation(Screen screen)
	{
		ScreenRoot screenScript = _screenDict[screen];
		screenScript.ResetAnimationPosition ();
	}
	

	void PushPreviousScreenToPreviousScreenList( Screen newScreen )
	{
		//push current screen to the previous list enless it is one of the initial screens
		//if it is initial screen, clear previous screenlist then add itself in
		if(!IsOverlay(newScreen))
		{
			if(_currentScreen.scene==null)
			{
				//when currentScene is None, means this is the first time calling show screen function through showInitialScreen function
				//do not add any screens into the list
				ClearPreviousScreenList();
			}
			else
			{
				//screens in the invalid list can not be transition back to
				if(!_currentScreen.CantTransitionBack)
				{
					previousScreenList.Add (_currentScreen);
				}
				else
				{
					ClearPreviousScreenList();
				}
				
				//if new screen is in the initial list, clear the list and make the newScene the first one, so it will always be the last screen you can transit to
				if(newScreen.ActiveOnLoad)
				{
					ClearPreviousScreenList();
				}
			}
			
		}
		
	}

	public string GetCurrentScreenName()
	{
		if(_currentScreen!=null)
		{
			return _currentScreen.Name;
		}
		else
		{
			return null;
		}
	}

	public void ClearPreviousScreenList()
	{
		if (previousScreenList != null)
		{
			previousScreenList.Clear();
		}
	}

	#endregion
}



