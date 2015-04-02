using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRoot : MonoBehaviour
{
	public AnimationClip inAnimation;
	public AnimationClip outAnimation;

    protected Canvas _canvas;
    private Animation anim;
    private Transform _threeDeeObjects;

    public const string DEFAULT_TRANSITION_IN = "Default_In";
    public const string DEFAULT_TRANSITION_OUT = "Default_Out";

    public event System.Action StartTransitionnInEvent;

    protected virtual void Awake()
	{
#if UNITY_EDITOR
		if(PlayerPrefs.GetString(UIManager.MasterScenePathKey)!=null)
		{
#endif
			var eventSystem = GetComponentInChildren<UnityEngine.EventSystems.EventSystem>();
			if ( eventSystem != null ) Destroy( eventSystem.gameObject );
			
			Transform rootCanvasTransform = this.transform;
			DebugUtil.Assert( rootCanvasTransform != null, "Unable to find root canvas! - " + name );

			_canvas = this.gameObject.GetComponent<Canvas>();

			DebugUtil.Assert( _canvas != null, "Unable to find canvas component! - " + name );
			
			// May not have this if no transitions setup
			//
			anim = this.gameObject.AddComponent<Animation>();
			if(inAnimation!=null)
			{
				anim.AddClip(inAnimation,inAnimation.name);
			}
			if(outAnimation!=null)
			{
				anim.AddClip(outAnimation,outAnimation.name);
			}

			Animator a=this.GetComponent<Animator>() as Animator;
			if(a!=null)
			{
				Destroy(a);
			}
			
			var camera=GetComponentInChildren<Camera>();
			Destroy( camera.gameObject );
			_canvas.worldCamera = Camera.main;
			
			_threeDeeObjects = transform.FindChild( "Canvas/3DObjects" );
			
			SetSceneActiveState( false );
			
			DebugUtil.Assert( name.StartsWith( "Screen_" ), "Invalid screen name! Structure must be \"Screen_SceneName\" " + name );
			
			string sceneName = name.Split( '_' )[1];
			

#if UNITY_EDITOR
		}
#endif
    }


    public IEnumerator _TransitionIn( string clipName )
    {
        if ( anim != null && inAnimation != null )
		{
			anim.Play( inAnimation.name );
            while ( anim.IsPlaying( clipName ) )
            {
                yield return null;
            }
        }
    }


    public IEnumerator _TransitionOut(  System.Action onFinishCB )
    {
        if ( anim != null && outAnimation != null )
        {
            anim.Play( outAnimation.name );
            while ( anim.IsPlaying( outAnimation.name ) )
            {
                yield return null;
            }
        }
        if ( onFinishCB != null ) onFinishCB();
    }


    public void SetSceneActiveState( bool isActive )
    {
		DebugUtil.Assert(_canvas!=null,"canvas is fuccccckckckkckckcking null");
        _canvas.enabled = isActive;
        Toggle3DObjects( isActive );
    }


    // Called before screen transition in has started
    //
    public virtual void OnPreTransitionIn() {}
    

    // Called when screen transition in has completed
    //
    public virtual void OnTransitionInComplete() {}
    

    // Called before screen is about to transition out
    //
    public virtual void OnPreTransitionOut() {}


    // Called after screen has transitioned out
    //
    public virtual void OnTransitionOutComplete() {}


    // Called immediatly once scene canvas renderer becomes enabled after transition
    //
    public virtual void OnSceneBecameVisible() {}


    // Called immediatly once scene canvas renderer becomes disabled after transition
    //
    public virtual void OnSceneBecameInvisible() {}


    public virtual void OnPreviousScreenTransitionedOut() {}


    // Default screen initializer
    public virtual IEnumerator Initialize()
    {
        yield break;
    }


    protected void PlayAnimation( string clipName )
    {
		DebugUtil.Assert( anim != null, string.Format( "Animation reference is null! {0}", name ) );
		DebugUtil.Assert( anim.GetClip( clipName ) != null, string.Format( "Animation comp has no clip namedd {0}", clipName ) );

        anim.Play( clipName );
    }


    private void Toggle3DObjects( bool active )
    {
        if ( _threeDeeObjects != null )
        {
            _threeDeeObjects.gameObject.SetActive( active );
        }
    }


    public virtual void BackButtonSelected()
    {
		//move background back, can be commented if we do not use scrolling background

    }
    

    public void ReparentGameObjectToCanvas( GameObject gameObject )
    {
        gameObject.transform.SetParent( _canvas.transform, true );
    }


    public void StartTranisitonInAnimationEvent()
    {
        if ( StartTransitionnInEvent != null ) StartTransitionnInEvent();
    }

	public void ResetAnimationPosition()
	{
		StartCoroutine (_ResetAnimationPosition());
	}

	IEnumerator _ResetAnimationPosition()
	{
		anim.Play();
		anim.Rewind ();
		yield return null;
		anim.Stop();
	}
}
