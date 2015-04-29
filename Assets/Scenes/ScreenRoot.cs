using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScreenRoot : MonoBehaviour
{
	public AnimationClip inAnimation;
	public AnimationClip outAnimation;

	public AnimationClip back_inAnim;
	public AnimationClip back_outAnim;

	public bool IsPopUp=false;

	public UnityEvent PreTransitionIn;
	public UnityEvent AfterTransitionIn;
	public UnityEvent PreTransitionOut;
	public UnityEvent AfterTransitionOut;

    protected Canvas _canvas;
    private Animation anim;
    private Transform _threeDeeObjects;


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
			anim.playAutomatically=false;
			if(inAnimation!=null)
			{
				inAnimation.legacy=true;
				anim.AddClip(inAnimation,inAnimation.name);
			}
			if(outAnimation!=null)
			{
				outAnimation.legacy=true;
				anim.AddClip(outAnimation,outAnimation.name);
			}
			if(back_inAnim!=null)
			{
				back_inAnim.legacy=true;
				anim.AddClip(back_inAnim,back_inAnim.name);
			}
			if(back_outAnim!=null)
			{
				back_outAnim.legacy=true;
				anim.AddClip(back_outAnim,back_outAnim.name);
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
			
//			string sceneName = name.Split( '_' )[1];
			

#if UNITY_EDITOR
		}
#endif
    }


    public IEnumerator _TransitionIn( bool playAnim ,bool isBackBtn=false)
    {
        if ( anim != null && inAnimation != null &&playAnim)
		{
			string animName=outAnimation.name;
			if(isBackBtn==true && back_inAnim!=null)
			{
				animName=back_inAnim.name;
			}
			anim.Play( animName );
			while ( anim.IsPlaying( animName ) )
			{
				yield return null;
			}
        }
    }


	public IEnumerator _TransitionOut(  System.Action onFinishCB ,bool playAnim,bool isBackBtn=false)
    {
        if ( anim != null && outAnimation != null &&playAnim )
        {
			string animName=outAnimation.name;
			if(isBackBtn==true && back_outAnim!=null)
			{
				animName=back_outAnim.name
			}
			anim.Play(animName);
			while ( anim.IsPlaying(animName) )
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
    public virtual void OnPreTransitionIn() 
	{
		if(PreTransitionIn!=null)
		{
			PreTransitionIn.Invoke();
		}
	}
    

    // Called when screen transition in has completed
    //
    public virtual void OnTransitionInComplete() 
	{
		if(AfterTransitionIn!=null)
		{
			AfterTransitionIn.Invoke();
		}
	}
    

    // Called before screen is about to transition out
    //
    public virtual void OnPreTransitionOut() 
	{
		if(PreTransitionOut!=null)
		{
			PreTransitionOut.Invoke();
		}
	}


    // Called after screen has transitioned out
    //
    public virtual void OnTransitionOutComplete() 
	{
		if(AfterTransitionOut!=null)
		{
			AfterTransitionOut.Invoke();
		}

	}


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
