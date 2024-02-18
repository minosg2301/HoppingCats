using UnityEngine;
using System;
using System.Collections.Generic;



public class TKPanRecognizer : TKAbstractGestureRecognizer
{
    public Action<TKPanRecognizer> gestureRecognizedEvent;
    public Action<TKPanRecognizer> gestureCompleteEvent;

    public Vector2 deltaTranslation;
    public float deltaTranslationCm;
    public Vector2 deltaTranslationFromRoot;
    public float deltaTranslationCmFromRoot;
    public int minimumNumberOfTouches = 1;
    public int maximumNumberOfTouches = 2;

    private float totalDeltaMovementInCm = 0f;
    private Vector2 _previousLocation;
    private float _minDistanceToPanCm, _maxPanDistanceCm, _maxMagnitude;
    private Vector2 _startPoint, _rootPoint;
    private Vector2 _endPoint;

    public Vector2 StartPoint => _startPoint;
    public Vector2 RootPoint => _rootPoint;
    public Vector2 CurrentPoint => _previousLocation;
    public Vector2 EndPoint => _endPoint;


    public TKPanRecognizer(float minPanDistanceCm = 0.5f, float maxPanDistanceCm = 1f)
    {
        _minDistanceToPanCm = minPanDistanceCm;
        _maxPanDistanceCm = maxPanDistanceCm;
        _maxMagnitude = _maxPanDistanceCm * TouchKit.instance.ScreenPixelsPerCm;
    }

    internal override void fireRecognizedEvent() => gestureRecognizedEvent?.Invoke(this);

    internal override bool TouchesBegan(List<TKTouch> touches)
    {
        // extra touches abort gesture
        if (_trackingTouches.Count + touches.Count > maximumNumberOfTouches)
        {
            state = TKGestureRecognizerState.FailedOrEnded;
            return false;
        }

        // add new or additional touches to gesture (allows for two or more touches to be added or removed without ending the pan gesture)
        if (state == TKGestureRecognizerState.Possible || ((state == TKGestureRecognizerState.Began || state == TKGestureRecognizerState.RecognizedAndStillRecognizing) && _trackingTouches.Count < maximumNumberOfTouches))
        {
            for (int i = 0; i < touches.Count; i++)
            {
                // only add touches in the Began phase
                if (touches[i].phase == TouchPhase.Began)
                {
                    _trackingTouches.Add(touches[i]);
                    _startPoint = touches[0].position;
                    _rootPoint = touches[0].position;

                    if (_trackingTouches.Count == maximumNumberOfTouches)
                        break;
                }
            } // end for

            if (_trackingTouches.Count >= minimumNumberOfTouches && _trackingTouches.Count <= maximumNumberOfTouches)
            {
                _previousLocation = TouchLocation();
                if (state != TKGestureRecognizerState.RecognizedAndStillRecognizing)
                {
                    totalDeltaMovementInCm = 0f;
                    state = TKGestureRecognizerState.Began;
                }
            }
        }

        return false;
    }

    internal override void touchesMoved(List<TKTouch> touches)
    {
        //do not engage with touch events if the number of touches is outside our desired constraints
        if (_trackingTouches.Count >= minimumNumberOfTouches && _trackingTouches.Count <= maximumNumberOfTouches)
        {
            var currentLocation = TouchLocation();
            deltaTranslation = currentLocation - _previousLocation;
            deltaTranslationCm = deltaTranslation.magnitude / TouchKit.instance.ScreenPixelsPerCm;
            _previousLocation = currentLocation;

            // update root point
            deltaTranslationFromRoot = currentLocation - _rootPoint;
            deltaTranslationCmFromRoot = deltaTranslationFromRoot.magnitude / TouchKit.instance.ScreenPixelsPerCm;
            if (deltaTranslationCmFromRoot > _maxPanDistanceCm)
                _rootPoint = currentLocation + (_rootPoint - currentLocation).normalized * _maxMagnitude;

            if (state == TKGestureRecognizerState.Began)
            {
                totalDeltaMovementInCm += deltaTranslationCm;

                if (Math.Abs(totalDeltaMovementInCm) >= _minDistanceToPanCm)
                {
                    state = TKGestureRecognizerState.RecognizedAndStillRecognizing;
                }
            }
            else
            {
                state = TKGestureRecognizerState.RecognizedAndStillRecognizing;
            }
        }
    }

    internal override void touchesEnded(List<TKTouch> touches)
    {
        _endPoint = TouchLocation();

        // remove any completed touches
        for (int i = 0; i < touches.Count; i++)
        {
            if (touches[i].phase == TouchPhase.Ended)
                _trackingTouches.Remove(touches[i]);
        }

        // if we still have a touch left continue. no touches means its time to reset
        if (_trackingTouches.Count >= minimumNumberOfTouches)
        {
            _previousLocation = TouchLocation();
            state = TKGestureRecognizerState.RecognizedAndStillRecognizing;
        }
        else
        {
            // if we had previously been recognizing fire our complete event
            if (state == TKGestureRecognizerState.RecognizedAndStillRecognizing)
            {
                gestureCompleteEvent?.Invoke(this);
            }

            state = TKGestureRecognizerState.FailedOrEnded;
        }
    }

    public override string ToString()
    {
        return string.Format("[{0}] state: {1}, location: {2}, deltaTranslation: {3}", this.GetType(), state, TouchLocation(), deltaTranslation);
    }

}
