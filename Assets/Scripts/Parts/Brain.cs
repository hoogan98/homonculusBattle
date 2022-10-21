using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : Part
{
    public GameObject giblet;
    public int maxGibs;

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

        transform.position = new Vector3(mousePos.x, mousePos.y, startZ);
        transform.rotation = Quaternion.identity;

        handler.SetBrain(gameObject);

        //handler.EndDraw();
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

    public override void FinishDraw(DrawParts drawingHandler)
    {
        base.FinishDraw(drawingHandler);

        drawingHandler.EndDraw();
    }

    public override void Die()
    {
        int gibCount = Random.Range(maxGibs / 2, maxGibs + 1);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        for (int i = 0; i < gibCount; i++) {
            Rigidbody2D newGibRb = Instantiate(giblet, transform.position, transform.rotation).GetComponent<Rigidbody2D>();
            newGibRb.AddForce(rb.velocity);
            newGibRb.AddTorque(rb.rotation);
        }

        base.Die();
    }

    
}
