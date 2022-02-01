using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muscle : Part
{
    public float strength = 2.5f;
    public float strengthRatio;
    
    private Vector3 _drawAnchor;
    //these are the base values to be updated in inspector
    // public override void Start()
    // {
    //     base.Start();
    //     float sizeMin = 0.1f;
    //     float sizeMax = 4f;
    // }
    
    public override void StartDraw()
    {
        throw new System.NotImplementedException();
    }

    public override void DrawingBehavior()
    {
        FollowMouseAnchor(_drawAnchor);
        EditSize();

        strength = transform.localScale.y / strengthRatio;
    }
}
