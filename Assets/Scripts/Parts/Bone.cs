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

    public override void FinishDraw(DrawParts drawingHandler)
    {
        if (transform.lossyScale.x == 0)
        {
            drawingHandler.RemovePart(this);
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, 1);
        GetComponent<DamageCheck>().boneScale = GetComponent<Renderer>().bounds.size.x;
        
        drawingHandler.EndDraw();
    }
}
