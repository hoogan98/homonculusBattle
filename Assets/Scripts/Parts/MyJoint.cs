using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyJoint : Part
{

    public HingeJoint2D hinge;

    public override void StartDraw(DrawParts handler)
    {
        Vector2 mousePos = Input.mousePosition;
        transform.position = new Vector3(mousePos.x, mousePos.y, -2);

        ratio = GetComponent<Transform>().localScale.x /
                    GetComponentInChildren<Renderer>().bounds.size.x;
    }

    public override void DrawingBehavior()
    {
        throw new System.NotImplementedException();
    }

    public override void FinishDraw(DrawParts drawingHandler)
    {
    }

    protected override void OnCollisionEnter2D(Collision2D collision) {
    }
}
