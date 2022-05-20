using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyJoint : Part
{
    // public Part[] bindingParts;
    public HingeJoint2D hinge;
    public Part connectedPart;

    public override void StartDraw(DrawParts handler)
    {
        Vector2 mousePos = Input.mousePosition;
        transform.position = new Vector3(mousePos.x, mousePos.y, -2);

        ratio = GetComponent<Transform>().localScale.x /
                    GetComponentInChildren<Renderer>().bounds.size.x;
    }

    public override void StartGame()
    {
        //maybe kill self if not connected to anything?
    }

    public override void DrawingBehavior()
    {
    }

    public override void FinishDraw(DrawParts drawingHandler)
    {
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
    }

    // public void AddConnection(Part p1, Part p2)
    // {
    //     if (bindingParts.Length == 0)
    //     {
    //         bindingParts = new Part[2];
    //     }

    //     bindingParts[0] = p1;
    //     bindingParts[1] = p2;
    // }

    // public void ReplaceConnectedBone(Bone newBone, Bone oldBone) {
    //     if (bindingParts[0].Equals(oldBone))
    //     {
    //         bindingParts[0] = newBone;
    //     }
    //     else
    //     {
    //         bindingParts[1] = newBone;
    //     }
    // }

    // public void AddConnection(Part p)
    // {
    //     if (bindingParts.Length == 0)
    //     {
    //         bindingParts = new Part[2];
    //     }

    //     if (bindingParts[0] == null)
    //     {
    //         bindingParts[0] = p;
    //     }
    //     else
    //     {
    //         bindingParts[1] = p;
    //     }
    // }
}
