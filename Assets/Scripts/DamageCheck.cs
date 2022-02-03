using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCheck : MonoBehaviour
{
    public float baseHealth;
    public List<Muscle> connectedMuscles;
    public List<Joint> connectedJoints;
    public float boneScale;
    public float minBoneX;

    public void Break()
    {
        if (this.gameObject.CompareTag("Bone"))
        {
            BoneSplit();
        }
        else if (this.gameObject.CompareTag("Muscle"))
        {
            MuscleSplit();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void StartGame()
    {
        this.baseHealth *= this.gameObject.transform.lossyScale.y;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float damage = collision.relativeVelocity.magnitude * collision.otherRigidbody.mass;
        Debug.Log(damage + " on " + this.gameObject.name);

        try
        {
            float otherHealth = collision.gameObject.GetComponent<DamageCheck>().baseHealth;
            if (otherHealth < this.baseHealth && otherHealth < damage)
            {
                Debug.Log("kill other attempt");
                collision.gameObject.GetComponent<DamageCheck>().Break();
                return;
            }
        }
        catch { }

        if (damage > baseHealth)
        {
            Break();
        }
    }

    //cut mr bones in two
    private void BoneSplit()
    {
        if ((this.transform.lossyScale.x / 2) < this.minBoneX)
        {
            Destroy(this.gameObject);
            return;
        }

        SpriteRenderer oldRend = this.gameObject.GetComponent<SpriteRenderer>();
        float offset = this.transform.lossyScale.x * this.boneScale;
        offset = offset / 4;
        Transform oldTrans = this.gameObject.transform;
        Joint[] oldJoints = this.gameObject.GetComponentsInChildren<Joint>();
        SpringJoint2D[] oldSprings = this.gameObject.GetComponents<SpringJoint2D>();

        GameObject bone1 = Instantiate(this.gameObject, oldTrans.position, oldTrans.rotation);
        bone1.transform.position += bone1.transform.TransformDirection(Vector3.right) * offset * -1;
        bone1.transform.localScale = new Vector3(oldTrans.localScale.x / 2, oldTrans.localScale.y, oldTrans.localScale.z);

        GameObject bone2 = Instantiate(this.gameObject, oldTrans.position, oldTrans.rotation);
        bone2.transform.position += bone2.transform.TransformDirection(Vector3.right) * offset;
        bone2.transform.localScale = new Vector3(oldTrans.localScale.x / 2, oldTrans.localScale.y, oldTrans.localScale.z);

        //kill the old joints because those are bad
        foreach (Transform t in bone1.GetComponentsInChildren<Transform>())
        {
            if (t.gameObject.CompareTag("Bone"))
            {
                continue;
            }
            Destroy(t.gameObject);
        }
        foreach (Transform t in bone2.GetComponentsInChildren<Transform>())
        {
            if (t.gameObject.CompareTag("Bone"))
            {
                continue;
            }
            Destroy(t.gameObject);
        }

        //kill old hinges too cause we remake those
        HingeJoint2D[] del1 = bone1.GetComponents<HingeJoint2D>();
        foreach (HingeJoint2D h in del1)
        {
            Destroy(h);
        }
        HingeJoint2D[] del2 = bone2.GetComponents<HingeJoint2D>();
        foreach (HingeJoint2D h in del2)
        {
            Destroy(h);
        }
        SpringJoint2D[] del3 = bone1.GetComponents<SpringJoint2D>();
        foreach (SpringJoint2D h in del3)
        {
            Destroy(h);
        }
        SpringJoint2D[] del4 = bone2.GetComponents<SpringJoint2D>();
        foreach (SpringJoint2D h in del4)
        {
            Destroy(h);
        }


        //assign the joint babies to foster bones
        foreach (Joint j in oldJoints)
        {
            GameObject joint = j.gameObject;

            GameObject parentBone = ClosestObject(bone1, bone2, joint.transform.position);

            joint.transform.parent = parentBone.transform;

            if (j.hinge != null)
            {
                HingeJoint2D hinge = j.hinge;

                HingeJoint2D newHinge = parentBone.AddComponent<HingeJoint2D>();
                newHinge.connectedBody = hinge.connectedBody;
                newHinge.connectedAnchor = hinge.connectedAnchor;
                newHinge.anchor = parentBone.transform.InverseTransformPoint(joint.transform.position);

                j.hinge = newHinge;
            }
        }

        //missing your mr bones after joint transfer? gotta update here
        foreach (Joint j in connectedJoints)
        {
            if (j == null)
            {
                continue;
            }
            GameObject joint = j.gameObject;

            GameObject parentBone = ClosestObject(bone1, bone2, joint.transform.position);

            if (j.hinge != null)
            {
                HingeJoint2D hinge = j.hinge;
                hinge.connectedBody = parentBone.GetComponent<Rigidbody2D>();
                hinge.connectedAnchor = parentBone.transform.InverseTransformPoint(joint.transform.position);
            }
        }

        //you are not the father of this muscle
        foreach (Muscle m in connectedMuscles)
        {
            if (m == null)
            {
                continue;
            }

            SpringJoint2D spring = m.spring;

            GameObject parentBone = ClosestObject(bone1, bone2, m.connectedAnchorJoint.transform.position);

            spring.connectedBody = parentBone.GetComponent<Rigidbody2D>();
            spring.connectedAnchor = parentBone.transform.InverseTransformPoint(m.connectedAnchorJoint.transform.position);
        }

        //update the spring joints in a broken boi
        foreach (SpringJoint2D sp in oldSprings)
        {
            Vector3 oldPoint = this.transform.TransformPoint(sp.anchor);
            GameObject parentBone = ClosestObject(bone1, bone2, oldPoint);

            SpringJoint2D newSpring = parentBone.AddComponent<SpringJoint2D>();
            newSpring.connectedBody = sp.connectedBody;
            newSpring.connectedAnchor = sp.connectedAnchor;
            newSpring.anchor = parentBone.transform.InverseTransformPoint(oldPoint);
            newSpring.autoConfigureDistance = false;
            newSpring.enableCollision = true;
        }

        Destroy(this.gameObject);
    }
    
    //split up them muscless
    private void MuscleSplit()
    {
        Muscle mb = GetComponent<Muscle>();

        if (mb == null)
        {
            return;
        }

        Destroy(mb.spring);
        Destroy(this.GetComponent<BoxCollider2D>());
        HingeJoint2D newHinge = this.gameObject.AddComponent<HingeJoint2D>();
        newHinge.anchor = this.transform.InverseTransformPoint(mb.anchorJoint.transform.position);
        newHinge.connectedAnchor = mb.anchorJoint.transform.parent.transform.InverseTransformPoint(mb.anchorJoint.transform.position);
        newHinge.connectedBody = mb.anchorJoint.transform.parent.gameObject.GetComponent<Rigidbody2D>();

        this.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        Destroy(mb);
    }

    //get the actual health of the boi
    public float GetHealth()
    {
        return this.baseHealth;
    }

    //figure out which child bone is closer to a certain point
    private GameObject ClosestObject(GameObject bone1, GameObject bone2, Vector3 point)
    {
        float dis1 = Vector3.Distance(bone1.transform.position, point);
        float dis2 = Vector3.Distance(bone2.transform.position, point);

        if (dis1 < dis2)
        {
            return bone1;
        }
        else
        {
            return bone2;
        }
    }
}
