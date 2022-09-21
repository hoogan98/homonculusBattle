using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyJoint : Part
{
    public HingeJoint2D hinge;
    public Part connectedPart;
    public GameObject spinner;

    public override void Start()
    {
    }

    public override void StartDraw(DrawParts drawingHandler)
    {
        //don't call this we don't need to
    }

    public void StartDraw()
    {
        // Vector2 mousePos = Input.mousePosition;
        // transform.position = new Vector3(mousePos.x, mousePos.y, startZ);

        GameObject.Instantiate(spinner, transform.position, transform.rotation);

        ratio = GetComponent<Transform>().localScale.x /
                    GetComponentInChildren<Renderer>().bounds.size.x;

        homonculus = transform.parent.parent.GetComponent<Homonculus>();
        homonculus.PlaySound(useSound);
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
}
