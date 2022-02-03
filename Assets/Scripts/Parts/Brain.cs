using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : Part
{
    public override void StartDraw(DrawParts handler)
    {
        if (handler.hasBrain)
        {
            handler.RemovePart(this);
            return;
        }
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = mousePos;
        transform.rotation = Quaternion.identity;

        handler.SetBrain(gameObject);
        
        handler.EndDraw();
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
