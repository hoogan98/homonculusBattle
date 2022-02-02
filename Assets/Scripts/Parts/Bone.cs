using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone : Part
{
    private Vector3 _drawAnchor;

    public override void StartDraw(DrawParts handler)
    {
        _drawAnchor = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 start = new Vector2(_drawAnchor.x + (GetComponent<Renderer>().bounds.size.x / 2), _drawAnchor.y);

        transform.position = start;
    }

    public override void DrawingBehavior()
    {
        FollowMouseAnchor(_drawAnchor);
        EditSize();
    }
}
