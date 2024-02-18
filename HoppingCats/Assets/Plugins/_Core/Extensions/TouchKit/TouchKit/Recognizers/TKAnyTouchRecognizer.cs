using UnityEngine;
using System;
using System.Collections.Generic;


public class TKAnyTouchRecognizer : TKAbstractGestureRecognizer
{
	public Action<TKAnyTouchRecognizer> onEnteredEvent;
	public Action<TKAnyTouchRecognizer> onExitedEvent;


	/// <summary>
	/// the contstructor ensures we have a frame to work with
	/// </summary>
	public TKAnyTouchRecognizer( TKRect frame )
	{
		alwaysSendTouchesMoved = true;
		boundaryFrame = frame;
	}

    /// <summary>
    /// 
    /// </summary>
    public TKAnyTouchRecognizer()
    {
        alwaysSendTouchesMoved = true;
        boundaryFrame = new TKRect(0, 0, Screen.width, Screen.height);
    }

	void onTouchEntered()
	{
		// fire the event if this is the first touch we are tracking
		if( _trackingTouches.Count == 1 && onEnteredEvent != null )
			onEnteredEvent( this );
	}


	void onTouchExited()
	{
		if( _trackingTouches.Count == 0 && onExitedEvent != null )
			onExitedEvent( this );
	}


	#region TKAbstractGestureRecognizer

	// we do nothing here. all events will be handled internally
	internal override void fireRecognizedEvent() {}


	internal override bool TouchesBegan( List<TKTouch> touches )
	{
		// grab the first touch that begins on us
		if( state == TKGestureRecognizerState.Possible )
		{
			for( int i = 0; i < touches.Count; i++ )
			{
				// only add touches in the Began phase
				if( touches[i].phase == TouchPhase.Began )
				{
					_trackingTouches.Add( touches[i] );
					state = TKGestureRecognizerState.RecognizedAndStillRecognizing;
					onTouchEntered();

					return true;
				}
			}
		}

		return false;
	}


	internal override void touchesMoved( List<TKTouch> touches )
	{
		for( int i = 0; i < touches.Count; i++ )
		{
			// check to see if the touch is in our frame
			var isTouchInFrame = IsTouchWithinBoundaryFrame( touches[i] );

			// are we already tracking this touch?
			var isTrackingTouch = _trackingTouches.Contains( touches[i] );

			// if we are tracking the touch and it is in frame we do nothing more
			if( isTrackingTouch && isTouchInFrame )
				continue;

			// if we are not tracking the touch and it is in our frame start tracking it
			if( !isTrackingTouch && isTouchInFrame )
			{
				_trackingTouches.Add( touches[i] );
				state = TKGestureRecognizerState.RecognizedAndStillRecognizing;
				//onTouchEntered();
			}
			// if we are tracking the touch and it exited the frame fire the onExitedEvent
			else if( isTrackingTouch && !isTouchInFrame )
			{
				_trackingTouches.Remove( touches[i] );
				state = TKGestureRecognizerState.FailedOrEnded;
				//onTouchExited();
			}
		}
	}


	internal override void touchesEnded( List<TKTouch> touches )
	{
		for( int i = 0; i < touches.Count; i++ )
		{
			if( touches[i].phase == TouchPhase.Ended && _trackingTouches.Contains( touches[i] ) )
			{
				_trackingTouches.Remove( touches[i] );
				state = TKGestureRecognizerState.FailedOrEnded;
				onTouchExited();
			}
		}
	}

	#endregion

}
