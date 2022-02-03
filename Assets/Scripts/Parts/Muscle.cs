using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muscle : Part
{
    public float strength = 2.5f;
    public float strengthRatio;
    public GameObject anchorJoint;
    public GameObject connectedAnchorJoint;
    public SpringJoint2D spring;
    public float healthScale;
    //muscleratio is just part's ratio

    private Vector3 _drawAnchor;
    private float _springStrength;
    private float _springStrengthDefault;
    private KeyCode _flexKey;
    private bool _hasKey;
    private bool _started;
    private bool _placedMuscle;
    private GameObject _firstBone;
    private Vector3 _startPoint;

    //these are the base values to be updated in inspector
    public override void Start()
    {
        base.Start();
        // float sizeMin = 0.1f;
        // float sizeMax = 4f;
        _placedMuscle = false;
        _hasKey = false;
        _started = false;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
    }
    
    void Update()
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

        if (!_hasKey)
        {
            return;
        }

        //i have no idea why I was resetting these anchor points before, it shouldn't need that right?
        if (Input.GetKeyDown(_flexKey))
        {
            spring.frequency = _springStrength;
            spring.distance /= 2;
            // spring.anchor = anchorJoint.transform.parent.InverseTransformPoint(anchorJoint.transform.position);
            // spring.connectedAnchor = connectedAnchorJoint.transform.parent.InverseTransformPoint(
            //                                 connectedAnchorJoint.transform.position);
        }
        else if (Input.GetKeyUp(_flexKey))
        {
            // this.spring.anchor = this.anchorJoint.transform.parent.InverseTransformPoint(this.anchorJoint.transform.position);
            // this.spring.connectedAnchor = this.connectedAnchorJoint.transform.parent.InverseTransformPoint(
            //                                 this.connectedAnchorJoint.transform.position);
            spring.distance *= 2;
            spring.frequency = _springStrengthDefault;
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

        RaycastHit2D[] allHits = Physics2D.RaycastAll(mousePos, Vector2.zero, Mathf.Infinity);
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        foreach (RaycastHit2D hit in allHits)
        {
            if (hit.collider.gameObject.CompareTag("Bone") || hit.collider.gameObject.CompareTag("Brain"))
            {
                hits.Add(hit);
            }
        }

        int len = hits.Count;

        if (!(hits.Count > 0))
        {
            handler.RemovePart(this);
            return;
        }

        if (!_placedMuscle)
        {
            _firstBone = hits[0].collider.gameObject;
            _placedMuscle = true;
            _startPoint = mousePos;
        }
        else
        {
            GameObject nextHit = hits[0].collider.gameObject;
            SpringJoint2D newSpring = _firstBone.AddComponent<SpringJoint2D>();
            newSpring.anchor = _firstBone.transform.InverseTransformPoint(_startPoint);
            newSpring.connectedBody = nextHit.GetComponent<Rigidbody2D>();
            newSpring.connectedAnchor = nextHit.transform.InverseTransformPoint(mousePos);
            newSpring.autoConfigureDistance = false;
            newSpring.enableCollision = true;

            //joints so the muslcess obey you
            GameObject joint1 = handler.CreateBasicJointAtPoint(_startPoint, _firstBone);
            GameObject joint2 = handler.CreateBasicJointAtPoint(mousePos, nextHit);

            newSpring.distance = Vector3.Distance(joint1.transform.position, joint2.transform.position);

            anchorJoint = joint1;
            connectedAnchorJoint = joint2;
            spring = newSpring;
            SetSpringStrength();

            //keep a reference to the original joint holder
            nextHit.GetComponent<DamageCheck>().connectedMuscles.Add(this);
            
            _placedMuscle = false;
            
            handler.EndDraw();
        }
    }

    public override void DrawingBehavior()
    {
        FollowMouseAnchor(_drawAnchor);
        EditSize();

        strength = transform.localScale.y / strengthRatio;
    }

    public override void FinishDraw(DrawParts drawingHandler)
    {
        throw new System.NotImplementedException();
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
        _hasKey = true;
        _flexKey = key;
    }

    public void StartGame()
    {
        //bug where halfway through drawing muscle on startup it stays
        if (anchorJoint == null || connectedAnchorJoint == null || spring == null)
        {
            Destroy(this.gameObject);
            return;
        }

        _started = true;
        spring.frequency = _springStrengthDefault;
        //uncomment if you ever figure out how to balance the muscle tearing without any impacts
        //float springHealth = this.GetComponent<DamageCheck>().GetHealth();
        //this.spring.breakForce = springHealth * this.healthScale;
    }
}