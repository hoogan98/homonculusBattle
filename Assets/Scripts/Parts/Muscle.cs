using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muscle : Part
{
    public float strength;
    public float yStrengthScale;
    public GameObject anchorJoint;
    public GameObject connectedAnchorJoint;
    public SpringJoint2D spring;
    public float yHealthScale;
    public float maxDistanceScale;
    public KeyCode _flexKey;
    public float springForceModifier;
    public float maxFlailDelay;
    public Sprite selectedSprite;

    private float _springStrength;
    private float _springStrengthDefault;
    private bool _started;
    private GameObject _firstBone;
    private Vector3 _drawAnchor;
    private float _flailDelay;
    private bool _flexed;

    //these are the base values to be updated in inspector
    public override void Start()
    {
        base.Start();
        _started = false;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        _flexed = false;
    }

    public override void LoadPart()
    {
        SetSpringStrength();

        base.LoadPart();
    }

    public void Update()
    {
        if (!_started)
        {
            return;
        }

        if (anchorJoint == null || connectedAnchorJoint == null)
        {
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
            return;
        }

        if (spring == null && _started)
        {
            FindNewJoint();
            return;
        }


        Vector3 joint1Pos = anchorJoint.transform.position;
        Vector3 joint2Pos = connectedAnchorJoint.transform.position;

        Vector3 direction = joint1Pos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;

        float muscleDisX = Vector3.Distance(joint1Pos, joint2Pos);
        float newScale = muscleDisX * ratio;
        Vector3 modScale = transform.localScale;
        modScale.x = newScale;
        transform.localScale = modScale;
        transform.position = (joint1Pos + joint2Pos) * 0.5f;

        if (brainConnected)
        {

            if (Input.GetKeyDown(_flexKey))
            {
                Flex();
            }
            else if (Input.GetKeyUp(_flexKey))
            {
                Relax();
            }
        }
        else
        {
            //flail around I guess
            if (_flailDelay < 0)
            {
                if (_flexed)
                {
                    Relax();
                }
                else
                {
                    Flex();
                }

                _flailDelay = Random.Range(0, maxFlailDelay);
            }
            else
            {
                _flailDelay -= Time.deltaTime;
            }
        }

        CheckBreak();
    }

    //i have no idea why I was resetting these anchor points before, it shouldn't need that right?
    private void Flex()
    {
        homonculus.PlayMuscleStretch();
        spring.frequency = _springStrength;
        spring.distance /= 2;
        _flexed = true;
        // spring.anchor = anchorJoint.transform.parent.InverseTransformPoint(anchorJoint.transform.position);
        // spring.connectedAnchor = connectedAnchorJoint.transform.parent.InverseTransformPoint(
        //                                 connectedAnchorJoint.transform.position);
    }

    private void Relax()
    {
        // this.spring.anchor = this.anchorJoint.transform.parent.InverseTransformPoint(this.anchorJoint.transform.position);
        // this.spring.connectedAnchor = this.connectedAnchorJoint.transform.parent.InverseTransformPoint(
        //                                 this.connectedAnchorJoint.transform.position);
        spring.distance *= 2;
        spring.frequency = _springStrengthDefault;
        _flexed = false;
    }

    private void CheckBreak()
    {
        if (baseHealth < (spring.reactionForce.magnitude / springForceModifier) ||
            Vector3.Distance(connectedAnchorJoint.transform.position, anchorJoint.transform.position) >
            spring.distance * maxDistanceScale)
        {
            Debug.Log(baseHealth + " muscleCheckBreak on: " + spring.reactionForce.magnitude / springForceModifier);
            Break();
        }
    }

    private void FindNewJoint()
    {
        GameObject parentBone = anchorJoint.transform.parent.gameObject;
        SpringJoint2D closest = null;
        float closestDis = -1;

        foreach (SpringJoint2D s in parentBone.GetComponents<SpringJoint2D>())
        {
            float dis = Vector3.Distance(anchorJoint.transform.position, parentBone.transform.TransformPoint(s.anchor));

            if (closest == null)
            {
                closest = s;
                closestDis = dis;
                continue;
            }

            if (dis < closestDis)
            {
                closestDis = dis;
                closest = s;
            }
        }

        if (closest != null)
        {
            spring = closest;
        }
    }

    public override void StartDraw(DrawParts handler)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        List<RaycastHit2D> hits = GetBonesAndBrainOverMouse();

        if (!(hits.Count > 0))
        {
            handler.RemovePart(this);
            return;
        }

        _firstBone = hits[0].collider.gameObject;
        _drawAnchor = mousePos;

        ratio = GetComponent<Transform>().localScale.x /
            GetComponentInChildren<Renderer>().bounds.size.x;
    }

    public override void DrawingBehavior()
    {
        FollowMouseAnchor(_drawAnchor);
        EditSize();

        strength = transform.localScale.y * yStrengthScale;
    }

    public override void FinishDraw(DrawParts handler)
    {
        base.FinishDraw(handler);

        if (transform.lossyScale.x == 0)
        {
            handler.RemovePart(this);
            handler.EndDraw();
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        GameObject nextHit = GetBonesAndBrainOverMouse()[0].collider.gameObject;

        if (nextHit.Equals(_firstBone))
        {
            handler.RemovePart(this);
            handler.EndDraw();
            return;
        }

        SpringJoint2D newSpring = _firstBone.AddComponent<SpringJoint2D>();
        newSpring.anchor = _firstBone.transform.InverseTransformPoint(_drawAnchor);
        newSpring.connectedBody = nextHit.GetComponent<Rigidbody2D>();
        newSpring.connectedAnchor = nextHit.transform.InverseTransformPoint(mousePos);
        newSpring.autoConfigureDistance = false;
        newSpring.enableCollision = true;

        //joints so the muslcess obey you
        GameObject joint1 = handler.CreateBasicJointAtPoint(_drawAnchor, _firstBone, this.gameObject);
        GameObject joint2 = handler.CreateBasicJointAtPoint(mousePos, nextHit, this.gameObject);

        //make sure to ignore collisions on the bones you are attached to
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), _firstBone.GetComponent<BoxCollider2D>());
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), nextHit.GetComponent<BoxCollider2D>());

        newSpring.distance = Vector3.Distance(joint1.transform.position, joint2.transform.position);

        anchorJoint = joint1;
        connectedAnchorJoint = joint2;
        spring = newSpring;
        SetSpringStrength();

        //keep a reference to the original joint holder
        nextHit.GetComponent<Part>().connectedMuscles.Add(this);

        transform.position = new Vector3(transform.position.x, transform.position.y, -1);

        baseHealth = transform.lossyScale.y * yHealthScale * baseHealth;

        handler.EndDraw();
    }

    private void SetSpringStrength()
    {
        if (strength < 0.5)
        {
            _springStrength = strength;
            _springStrengthDefault = strength / 2;
        }
        else
        {
            _springStrength = strength;
            _springStrengthDefault = (strength - 0.5f) / 2;
        }
    }

    public void SetFlexKey(KeyCode key)
    {
        GetComponentInChildren<SpriteRenderer>().sprite = selectedSprite;
        _flexKey = key;
    }

    public override void StartGame()
    {
        //bug where halfway through drawing muscle on startup it stays
        if (anchorJoint == null || connectedAnchorJoint == null || spring == null)
        {
            Destroy(gameObject);
            return;
        }

        _started = true;
        spring.frequency = _springStrengthDefault;

        //uncomment if you ever figure out how to balance the muscle tearing without any impacts
        //float springHealth = this.GetComponent<DamageCheck>().GetHealth();
        //this.spring.breakForce = springHealth * this.healthScale;
    }

    public override void Break()
    {
        homonculus.PlayMuscleBreak();
        Destroy(spring);
        Destroy(GetComponent<BoxCollider2D>());
        HingeJoint2D newHinge = gameObject.AddComponent<HingeJoint2D>();
        newHinge.anchor = transform.InverseTransformPoint(anchorJoint.transform.position);
        newHinge.connectedAnchor =
            anchorJoint.transform.parent.transform.InverseTransformPoint(anchorJoint.transform.position);
        newHinge.connectedBody = anchorJoint.transform.parent.gameObject.GetComponent<Rigidbody2D>();

        GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        Destroy(this);
        transform.parent.GetComponent<Homonculus>().ReportDestroyedPartAndRecalc(this);
    }
}