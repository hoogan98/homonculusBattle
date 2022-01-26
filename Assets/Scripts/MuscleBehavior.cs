using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuscleBehavior : MonoBehaviour
{
    public GameObject anchorJoint;
    public GameObject connectedAnchorJoint;
    public float muscleScale;
    public SpringJoint2D spring;
    public float healthScale;

    private float springStrength;
    private float springStrengthDefault;
    private KeyCode flexKey;
    private bool hasKey;
    private bool started;

    void Start()
    {
        this.hasKey = false;
        this.started = false;
    }

    void Update()
    {
        if (!this.started)
        {
            return;
        }
        else if (this.anchorJoint == null || this.connectedAnchorJoint == null)
        {
            this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
            return;
        }
        else if (this.spring == null && this.started)
        {
            FindNewJoint();
            return;
        }
        else
        {
            this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        }

        Vector3 joint1Pos = this.anchorJoint.transform.position;
        Vector3 joint2Pos = this.connectedAnchorJoint.transform.position;

        Vector2 direction = joint1Pos - this.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        this.transform.rotation = rotation;

        float mouseDisX = Vector3.Distance(joint1Pos, joint2Pos);
        float newScale = mouseDisX * this.muscleScale;
        Vector3 modScale = this.transform.localScale;
        modScale.x = newScale;
        this.transform.localScale = modScale;

        Vector2 newPos = (joint1Pos + joint2Pos) * 0.5f;
        this.transform.position = newPos;

        if (!this.hasKey)
        {
            
            return;
        }

        if (Input.GetKeyDown(this.flexKey))
        {
            this.spring.frequency = this.springStrength;
            this.spring.distance = this.spring.distance / 2;
            this.spring.anchor = this.anchorJoint.transform.parent.InverseTransformPoint(this.anchorJoint.transform.position);
            this.spring.connectedAnchor = this.connectedAnchorJoint.transform.parent.InverseTransformPoint(
                                            this.connectedAnchorJoint.transform.position);
        }
        else if (Input.GetKeyUp(this.flexKey))
        {
            this.spring.anchor = this.anchorJoint.transform.parent.InverseTransformPoint(this.anchorJoint.transform.position);
            this.spring.connectedAnchor = this.connectedAnchorJoint.transform.parent.InverseTransformPoint(
                                            this.connectedAnchorJoint.transform.position);
            this.spring.distance = this.spring.distance * 2;
            this.spring.frequency = this.springStrengthDefault;
        }
    }

    public void SetFlexKey(KeyCode key)
    {
        this.hasKey = true;
        this.flexKey = key;
    }

    public void StartGame()
    {
        //bug where halfway through drawing muscle on startup it stays
        if (this.anchorJoint == null || this.connectedAnchorJoint == null || this.spring == null)
        {
            Destroy(this.gameObject);
            return;
        }

        this.started = true;
        this.spring.frequency = this.springStrengthDefault;
        //uncomment if you ever figure out how to balance the muscle tearing without any impacts
        //float springHealth = this.GetComponent<DamageCheck>().GetHealth();
        //this.spring.breakForce = springHealth * this.healthScale;
    }

    public void SetSpringStrength(float strength)
    {
        if (strength < 0.5)
        {
            this.springStrength = strength;
            this.springStrengthDefault = strength / 2;
        }
        else
        {
            this.springStrength = strength;
            this.springStrengthDefault = (strength - 0.5f) / 2;
        }
    }

    private void FindNewJoint()
    {
        GameObject parentBone = this.anchorJoint.transform.parent.gameObject;
        SpringJoint2D closest = null;
        float closestDis = -1;

        foreach (SpringJoint2D s in parentBone.GetComponents<SpringJoint2D>())
        {
            float dis = Vector3.Distance(this.anchorJoint.transform.position, parentBone.transform.TransformPoint(s.anchor));

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
            this.spring = closest;
        }
    }
}
