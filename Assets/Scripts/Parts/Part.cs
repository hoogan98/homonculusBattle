using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public abstract class Part : MonoBehaviour
{
    public float ratio;
    public float sizeMin;
    public float sizeMax;
    public List<MyJoint> connectedJoints;
    public List<Muscle> connectedMuscles;
    public float baseHealth;
    public bool brainConnected;
    public bool visited;
    public AudioClip creationSound;

    protected Homonculus homonculus;

    private float bigHitThreshold = 7f;
    private float minSoundDamage = 3f;

    public virtual void Start()
    {
        if (connectedJoints == null)
        {
            connectedJoints = new List<MyJoint>();
        }

        if (connectedMuscles == null)
        {
            connectedMuscles = new List<Muscle>();
        }

        brainConnected = false;
        visited = false;

        homonculus = transform.parent.GetComponent<Homonculus>();
    }

    public abstract void StartGame();

    public virtual void LoadPart()
    {
        Destroy(GetComponent<DrawingBehavior>());
        GetComponent<BoxCollider2D>().isTrigger = false;
        StartGame();

        return;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        float damage = collision.relativeVelocity.magnitude * collision.otherRigidbody.mass;

        HitSoundHandler(damage, collision.gameObject.CompareTag("Ground"));

        if (damage > baseHealth)
        {
            Debug.Log("Breaking on: " + damage);
            Break();
        }
    }

    protected virtual void HitSoundHandler(float damage, bool isGround)
    {
        if (damage < minSoundDamage)
        {
            return;
        }

        if (isGround)
        {
            homonculus.PlayStep();
        }
        else if (damage > bigHitThreshold)
        {
            homonculus.PlayBigHit();
        }
        else
        {
            homonculus.PlayLightHit();
        }
    }

    public abstract void StartDraw(DrawParts drawingHandler);
    public abstract void DrawingBehavior();
    public virtual void FinishDraw(DrawParts drawingHandler) {
        homonculus = transform.parent.GetComponent<Homonculus>();
        homonculus.PlaySound(creationSound);
    }

    protected void FollowMouseAnchor(Vector3 anchor)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;

        float mouseDisX = Vector3.Distance(mousePos, anchor);
        float newScale = mouseDisX * ratio;
        Vector3 modScale = transform.localScale;
        modScale.x = newScale;
        transform.localScale = modScale;

        Vector2 newPos = (Vector2)((anchor + mousePos) * 0.5f);
        transform.position = (Vector3)new Vector3(newPos.x, newPos.y, 1);
    }

    protected List<RaycastHit2D> GetBonesAndBrainOverMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D[] allHits = Physics2D.RaycastAll(mousePos, Vector2.zero, Mathf.Infinity);
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        foreach (RaycastHit2D hit in allHits)
        {
            if (hit.collider.gameObject.CompareTag("Bone") || hit.collider.gameObject.CompareTag("Brain"))
            {
                hits.Add(hit);
            }
        }

        return hits;
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
