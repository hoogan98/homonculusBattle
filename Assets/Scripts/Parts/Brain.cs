using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        Debug.Log("running scene reset");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        base.Die();
    }
}
