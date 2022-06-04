using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyJoint : Part
{
    public HingeJoint2D hinge;
    public Part connectedPart;

    private Animation startAnim;

    public override void Start()
    {
    }

    public override void StartDraw(DrawParts handler)
    {
        Vector2 mousePos = Input.mousePosition;
        transform.position = new Vector3(mousePos.x, mousePos.y, -2);

        ratio = GetComponent<Transform>().localScale.x /
                    GetComponentInChildren<Renderer>().bounds.size.x;

        startAnim = GetComponent<Animation>();
        startAnim.wrapMode = WrapMode.Once;
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
        GetComponent<AudioSource>().PlayOneShot(creationSound);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
    }
}
