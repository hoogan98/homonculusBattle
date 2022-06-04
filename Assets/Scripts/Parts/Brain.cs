using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : Part
{
    public override void StartDraw(DrawParts handler)
    {
        ratio = GetComponent<Transform>().localScale.x /
                            GetComponentInChildren<Renderer>().bounds.size.x;

        if (handler.hasBrain)
        {
            handler.RemovePart(this);
            return;
        }

        brainConnected = true;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = new Vector3(mousePos.x, mousePos.y, 1);
        transform.rotation = Quaternion.identity;

        handler.SetBrain(gameObject);

        handler.EndDraw();
    }

    public override void StartGame()
    {
        //maybe kill other brains here?
        transform.parent.GetComponent<Homonculus>().StartGame();
    }

    public override void LoadPart()
    {
        base.LoadPart();

        GetComponent<Rigidbody2D>().gravityScale = 1;
    }

    public override void DrawingBehavior()
    {
    }

    protected override void OnCollisionEnter2D(Collision2D collision) {
    }
}
