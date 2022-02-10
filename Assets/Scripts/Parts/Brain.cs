using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : Part
{
    public override void StartDraw(DrawParts handler, float r)
    {
        ratio = r;
        if (handler.hasBrain)
        {
            handler.RemovePart(this);
            return;
        }
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = new Vector3(mousePos.x, mousePos.y, 0);
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
    }
}
