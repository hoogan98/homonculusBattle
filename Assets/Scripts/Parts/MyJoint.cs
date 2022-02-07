using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyJoint : Part
{
    
    public HingeJoint2D hinge;
    
    public override void StartDraw(DrawParts handler, float r)
    {
        ratio = r;
        Vector2 mousePos = Input.mousePosition;
        transform.position = new Vector3(mousePos.x, mousePos.y, -1);
    }

    public override void DrawingBehavior()
    {
        throw new System.NotImplementedException();
    }

    public override void FinishDraw(DrawParts drawingHandler)
    {
        throw new System.NotImplementedException();
    }
}
