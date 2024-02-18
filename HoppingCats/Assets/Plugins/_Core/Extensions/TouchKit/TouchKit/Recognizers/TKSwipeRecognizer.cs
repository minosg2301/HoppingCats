using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Flags]
public enum TKSwipeDirection
{
    Left = (1 << 0),
    Right = (1 << 1),
    Up = (1 << 2),
    Down = (1 << 3),

    UpLeft = (1 << 4),
    DownLeft = (1 << 5),
    UpRight = (1 << 6),
    DownRight = (1 << 7),

    Horizontal = (Left | Right),
    Vertical = (Up | Down),
    Cardinal = (Horizontal | Vertical),

    DiagonalUp = (UpLeft | UpRight),
    DiagonalDown = (DownLeft | DownRight),
    DiagonalLeft = (UpLeft | DownLeft),
    DiagonalRight = (UpRight | DownRight),
    Diagonal = (DiagonalUp | DiagonalDown),

    RightSide = (Right | DiagonalRight),
    LeftSide = (Left | DiagonalLeft),
    TopSide = (Up | DiagonalUp),
    BottomSide = (Down | DiagonalDown),

    All = (Horizontal | Vertical | Diagonal)
}

public class TKSwipeRecognizer : TKAbstractGestureRecognizer
{
    /// <summary>
    /// The event that fires when a swipe is recognized.
    /// </summary>
    public Action<TKSwipeRecognizer> gestureRecognizedEvent;

    /// <summary>
    /// The maximum amount of time for the motion to be considered a swipe.
    /// Setting to 0f will disable the time restriction completely.
    /// </summary>
    public float timeToSwipe = 0.5f;

    /// <summary>
    /// The velocity of the swipe, in centimeters based on the screen resolution
    /// and pixel density, if available.
    /// </summary>
    public float swipeVelocity { get; private set; }

    /// <summary>
    /// The direction that the swipe was made in. Possibilities include the four
    /// cardinal directions and the four diagonal directions.
    /// </summary>
    public TKSwipeDirection completedSwipeDirection { get; private set; }

    /// <summary>
    /// The minimum number of simultaneous touches (fingers) on the screen to trigger
    /// this swipe recognizer. Default is 1.
    /// </summary>
    public int minimumNumberOfTouches = 1;

    /// <summary>
    /// The maximum number of simultaneous touches (fingers) on the screen to trigger
    /// this swipe recognizer. Default is 2.
    /// </summary>
    public int maximumNumberOfTouches = 2;

    /// <summary>
    /// If true, will trigger on the frame that the criteria for a swipe are first met.
    /// If false, will only trigger on completion of the motion, when the touch is lifted.
    /// </summary>
    public bool triggerWhenCriteriaMet = true;
    public bool checkAtStartTime = true;

    /// <summary>
    /// The minimum distance in centimeters that the gesture has to make to be considered
    /// a proper swipe, based on resolution and pixel density. Default is 2cm.
    /// </summary>
    private float _minimumDistance = 2f;

    /// <summary>
    /// The maximum distance in centimeters that the gesture has to make to be considered 
    /// a proper swipe.
    /// </summary>
    private float _maximumDistance = 10f;

    /// <summary>
    /// The individual points that make up the gesture, recorded every frame from when a
    /// finger is first pressed to the screen until it's lifted. Only tracks the first touch
    /// on the screen, in the case of multiple touches.
    /// </summary>
    private List<Vector2> _points = new List<Vector2>();

    /// <summary>
    /// The time that the gesture started. Is used to determine if the time limit has been
    /// passed, and whether to ignore further checks.
    /// </summary>
    private float _startTime;
    private float _moveTime;

    /// <summary>
    /// The first touch point in the gesture.
    /// </summary>
    public Vector2 StartPoint => _points.FirstOrDefault();

    /// <summary>
    /// The last touch point in the gesture.
    /// </summary>
    public Vector2 EndPoint => _points.LastOrDefault();

    /// <summary>
    /// The pre last touch point in the gesture.
    /// </summary>
    public Vector2 PrevEndPoint => _points.Count > 1 ? _points[_points.Count - 2] : Vector2.zero;

    public TKSwipeRecognizer() : this(2f, 10f)
    { }

    public TKSwipeRecognizer(float minimumDistanceCm, float maximumDistanceCm)
    {
        _minimumDistance = minimumDistanceCm;
        _maximumDistance = maximumDistanceCm;
    }

    private bool CheckForSwipeCompletion(TKTouch touch)
    {
        return checkAtStartTime
        ? CheckForSwipeCompletionStartPoint(touch)
        : CheckForSwipeCompletionEndPoint(touch);
    }

    private bool CheckForSwipeCompletionEndPoint(TKTouch touch)
    {
        // if we have a time stipulation and we exceeded it stop listening for swipes, fail
        if (timeToSwipe > 0.0f && (Time.time - _moveTime) > timeToSwipe)
            return false;

        // if we don't have at least two points to test yet, then fail
        if (_points.Count < 2)
            return false;

        // the ideal distance in pixels from the start to the finish
        float idealDistance = Vector2.Distance(PrevEndPoint, EndPoint);

        // the ideal distance in centimeters, based on the screen pixel density
        float idealDistanceCM = idealDistance / TouchKit.instance.ScreenPixelsPerCm;

        // if the distance moved in cm was less than the minimum,
        if (idealDistanceCM < _minimumDistance)
            return false;

        // the speed in cm/s of the swipe
        swipeVelocity = idealDistanceCM / (Time.time - _moveTime);

        // turn the slope of the ideal swipe line into an angle in degrees
        Vector2 v2 = (EndPoint - PrevEndPoint).normalized;
        float swipeAngle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
        if (swipeAngle < 0)
            swipeAngle = 360 + swipeAngle;
        swipeAngle = 360 - swipeAngle;

        // depending on the angle of the line, give a logical swipe direction
        CalculateSwipeDirection(swipeAngle);

        return true;
    }

    private bool CheckForSwipeCompletionStartPoint(TKTouch touch)
    {
        // if we have a time stipulation and we exceeded it stop listening for swipes, fail
        if (timeToSwipe > 0.0f && (Time.time - _startTime) > timeToSwipe)
            return false;

        // if we don't have at least two points to test yet, then fail
        if (_points.Count < 2)
            return false;

        // the ideal distance in pixels from the start to the finish
        float idealDistance = Vector2.Distance(StartPoint, EndPoint);

        // the ideal distance in centimeters, based on the screen pixel density
        float idealDistanceCM = idealDistance / TouchKit.instance.ScreenPixelsPerCm;

        // if the distance moved in cm was less than the minimum,
        if (idealDistanceCM < _minimumDistance || idealDistanceCM > _maximumDistance)
            return false;

        // add up distances between all points sampled during the gesture to get the real distance
        float realDistance = 0f;
        for (int i = 1; i < _points.Count; i++)
            realDistance += Vector2.Distance(_points[i], _points[i - 1]);

        // if the real distance is 10% greater than the ideal distance, then fail
        // this weeds out really irregular "lines" and curves from being considered swipes
        if (realDistance > idealDistance * 1.1f)
            return false;

        // the speed in cm/s of the swipe
        swipeVelocity = idealDistanceCM / (Time.time - _startTime);

        // turn the slope of the ideal swipe line into an angle in degrees
        Vector2 v2 = (EndPoint - StartPoint).normalized;
        float swipeAngle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
        if (swipeAngle < 0)
            swipeAngle = 360 + swipeAngle;
        swipeAngle = 360 - swipeAngle;

        // depending on the angle of the line, give a logical swipe direction
        CalculateSwipeDirection(swipeAngle);

        return true;
    }

    private void CalculateSwipeDirection(float swipeAngle)
    {
        if (swipeAngle >= 292.5f && swipeAngle <= 337.5f)
            completedSwipeDirection = TKSwipeDirection.UpRight;
        else if (swipeAngle >= 247.5f && swipeAngle <= 292.5f)
            completedSwipeDirection = TKSwipeDirection.Up;
        else if (swipeAngle >= 202.5f && swipeAngle <= 247.5f)
            completedSwipeDirection = TKSwipeDirection.UpLeft;
        else if (swipeAngle >= 157.5f && swipeAngle <= 202.5f)
            completedSwipeDirection = TKSwipeDirection.Left;
        else if (swipeAngle >= 112.5f && swipeAngle <= 157.5f)
            completedSwipeDirection = TKSwipeDirection.DownLeft;
        else if (swipeAngle >= 67.5f && swipeAngle <= 112.5f)
            completedSwipeDirection = TKSwipeDirection.Down;
        else if (swipeAngle >= 22.5f && swipeAngle <= 67.5f)
            completedSwipeDirection = TKSwipeDirection.DownRight;
        else // swipeAngle >= 337.5f || swipeAngle <= 22.5f
            completedSwipeDirection = TKSwipeDirection.Right;
    }

    internal override void fireRecognizedEvent()
    {
        gestureRecognizedEvent?.Invoke(this);
    }

    internal override bool TouchesBegan(List<TKTouch> touches)
    {
        if (state == TKGestureRecognizerState.Possible)
        {
            // add any touches on screen
            for (int i = 0; i < touches.Count; i++)
                _trackingTouches.Add(touches[i]);

            // if the number of touches is within our constraints, begin tracking
            if (_trackingTouches.Count >= minimumNumberOfTouches && _trackingTouches.Count <= maximumNumberOfTouches)
            {
                // reset everything
                _points.Clear();
                _points.Add(touches[0].position);

                _startTime = Time.time;
                state = TKGestureRecognizerState.Began;
            }
        }
        return false;
    }

    internal override void touchesMoved(List<TKTouch> touches)
    {
        // only bother doing anything if we haven't recognized or failed yet
        if (state == TKGestureRecognizerState.Began)
        {
            _points.Add(touches[0].position);
            _moveTime = Time.time;

            // if we're triggering when the criteria is met, then check for completion every frame
            if (triggerWhenCriteriaMet && CheckForSwipeCompletion(touches[0]))
                state = TKGestureRecognizerState.Recognized;
        }
    }

    internal override void touchesEnded(List<TKTouch> touches)
    {
        // if we haven't recognized or failed yet
        if (state == TKGestureRecognizerState.Began)
        {
            _points.Add(touches[0].position);

            // last frame, one last check- recognized or fail
            if (CheckForSwipeCompletion(touches[0]))
                state = TKGestureRecognizerState.Recognized;
            else
                state = TKGestureRecognizerState.FailedOrEnded;
        }
    }

    public override string ToString()
    {
        return string.Format("{0}, swipe direction: {1}, swipe velocity: {2}, start point: {3}, end point: {4}",
            base.ToString(), completedSwipeDirection, swipeVelocity, StartPoint, EndPoint);
    }
}
