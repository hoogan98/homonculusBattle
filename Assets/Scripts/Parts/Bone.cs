using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone : Part
{
    public float minX;

    private Vector3 _drawAnchor;

    public override void StartDraw(DrawParts handler, float r)
    {
        ratio = r;
        _drawAnchor = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 start = new Vector3(_drawAnchor.x + (GetComponent<Renderer>().bounds.size.x / 2), _drawAnchor.y, 2);

        transform.position = start;
    }

    public override void DrawingBehavior()
    {
        FollowMouseAnchor(_drawAnchor);
        EditSize();
    }

    public override void FinishDraw(DrawParts drawingHandler)
    {
        if (transform.lossyScale.x == 0)
        {
            drawingHandler.RemovePart(this);
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, 1);
        GetComponent<Part>().ratio = GetComponent<Renderer>().bounds.size.x;

        drawingHandler.EndDraw();
    }

    private GameObject ClosestObject(GameObject bone1, GameObject bone2, Vector3 point)
    {
        float dis1 = Vector3.Distance(bone1.transform.position, point);
        float dis2 = Vector3.Distance(bone2.transform.position, point);

        if (dis1 < dis2)
        {
            return bone1;
        }

        return bone2;
    }

    public override void Break()
    {
        if ((transform.lossyScale.x / 2) < minX)
        {
            Destroy(gameObject);
            return;
        }

        float offset = transform.lossyScale.x * ratio;
        offset /= 4;
        Transform oldTrans = transform;
        MyJoint[] oldJoints = gameObject.GetComponentsInChildren<MyJoint>();
        SpringJoint2D[] oldSprings = gameObject.GetComponents<SpringJoint2D>();

        GameObject bone1 = Instantiate(gameObject, oldTrans.position, oldTrans.rotation);
        bone1.transform.position += bone1.transform.TransformDirection(Vector3.right) * offset * -1;
        bone1.transform.localScale =
            new Vector3(oldTrans.localScale.x / 2, oldTrans.localScale.y, oldTrans.localScale.z);

        GameObject bone2 = Instantiate(gameObject, oldTrans.position, oldTrans.rotation);
        bone2.transform.position += bone2.transform.TransformDirection(Vector3.right) * offset;
        bone2.transform.localScale =
            new Vector3(oldTrans.localScale.x / 2, oldTrans.localScale.y, oldTrans.localScale.z);

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

        //kill old hinges/springs too cause we remake those
        List<Joint2D> del = new List<Joint2D>();
        del.AddRange(bone1.GetComponents<HingeJoint2D>());
        del.AddRange(bone2.GetComponents<HingeJoint2D>());
        del.AddRange(bone1.GetComponents<SpringJoint2D>());
        del.AddRange(bone2.GetComponents<SpringJoint2D>());
        foreach (Joint2D h in del)
        {
            Destroy(h);
        }

        //assign the joint babies to foster bones
        foreach (MyJoint j in oldJoints)
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
        foreach (MyJoint j in connectedJoints)
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
            spring.connectedAnchor =
                parentBone.transform.InverseTransformPoint(m.connectedAnchorJoint.transform.position);
        }

        //update the spring joints in a broken boi
        foreach (SpringJoint2D sp in oldSprings)
        {
            Vector3 oldPoint = transform.TransformPoint(sp.anchor);
            GameObject parentBone = ClosestObject(bone1, bone2, oldPoint);

            SpringJoint2D newSpring = parentBone.AddComponent<SpringJoint2D>();
            newSpring.connectedBody = sp.connectedBody;
            newSpring.connectedAnchor = sp.connectedAnchor;
            newSpring.anchor = parentBone.transform.InverseTransformPoint(oldPoint);
            newSpring.autoConfigureDistance = false;
            newSpring.enableCollision = true;
        }

        Destroy(gameObject);
    }
}