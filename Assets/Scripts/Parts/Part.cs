using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Part : MonoBehaviour
{
    public float ratio;
    public float sizeMin;
    public float sizeMax;
    public List<MyJoint> connectedJoints;
    public List<Muscle> connectedMuscles;
    public float baseHealth;

    public virtual void Start()
    {
        ratio = GetComponent<Transform>().localScale.x / GetComponent<Renderer>().bounds.size.x;
        connectedJoints = new List<MyJoint>();
        connectedMuscles = new List<Muscle>();
    }
    
    public void StartGame()
    {
        baseHealth *= gameObject.transform.lossyScale.y;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        float damage = collision.relativeVelocity.magnitude * collision.otherRigidbody.mass;
        Debug.Log(damage + " on " + gameObject.name);

        try
        {
            float otherHealth = collision.gameObject.GetComponent<Part>().baseHealth;
            if (otherHealth < baseHealth && otherHealth < damage)
            {
                Debug.Log("kill other attempt");
                collision.gameObject.GetComponent<Part>().Break();
                return;
            }
        }
        catch
        {
            Debug.Log("error in trying to kill another part");
        }

        if (damage > baseHealth)
        {
            Break();
        }
    }

    public abstract void StartDraw(DrawParts drawingHandler);
    public abstract void DrawingBehavior();
    public abstract void FinishDraw(DrawParts drawingHandler);

    protected void FollowMouseAnchor(Vector3 anchor)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;

        float mouseDisX = Vector3.Distance(mousePos, anchor);
        float newScale = mouseDisX * ratio;
        Vector3 modScale = transform.localScale;
        modScale.x = newScale;
        transform.localScale = modScale;

        Vector2 newPos = (Vector2)((anchor + mousePos) * 0.5f);
        transform.position = (Vector3)newPos;
    }

    protected void EditSize()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (transform.localScale.y <= sizeMin)
            {
                return;
            }

            Vector3 t = transform.localScale;
            transform.localScale = new Vector3(t.x, t.y - sizeMin, t.z);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (transform.localScale.y >= sizeMax)
            {
                return;
            }

            Vector3 t = transform.localScale;
            transform.localScale = new Vector3(t.x, t.y + sizeMin, t.z);
        }
    }

    public virtual void Break()
    {
        Destroy(gameObject);
    }
}
