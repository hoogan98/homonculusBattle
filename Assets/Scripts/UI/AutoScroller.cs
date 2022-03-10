using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoScroller : MonoBehaviour
{
    public ScrollRect scrollRect;

    public float travelTime;
    public float timeOffset;

    private float _startTime;
    private bool _goingDown = true;
    
    // Start is called before the first frame update
    void Start()
    {
        _startTime = Time.time + timeOffset;
    }

    void Update()
    {
        var pos = _goingDown ? Vector3.Slerp(Vector3.up, Vector3.zero, (Time.time - _startTime) / travelTime) : Vector3.Slerp(Vector3.zero, Vector3.up, (Time.time - _startTime) / travelTime);
        scrollRect.normalizedPosition = pos;

        if (scrollRect.normalizedPosition == Vector2.zero && _goingDown) {
            _goingDown = false;
            _startTime = Time.time + timeOffset;
        } else if (scrollRect.normalizedPosition == Vector2.up && !_goingDown) {
            _goingDown = true;
            _startTime = Time.time + timeOffset;
        }
    }
}
