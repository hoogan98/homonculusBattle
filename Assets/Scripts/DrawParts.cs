using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawParts : MonoBehaviour
{
    public GameObject Bone;
    public GameObject Joint;
    public GameObject Muscle;
    public GameObject Brain;
    public Camera GameCam;
    public Text drawModeLabel;
    public float muscleStrengthMax;
    public float muscleStrengthMin;
    public float boneSizeMin;
    public float boneSizeMax;
    public bool isP1;
    public bool isActive;

    private bool DeleteMode = false;
    private Vector3 startPoint;
    private GameObject newBone;
    private float boneRatio;
    private List<GameObject> drawnParts;
    private enum DrawMode
    {
        Brain = 0,
        Bone = 1,
        Muscle = 2,
        KeyBind = 3
    };
    private DrawMode drawmode;
    private bool hasBrain;

    //muscle stuff
    private bool drawingMuscle;
    private GameObject muscleFirstBone;
    private GameObject newMuscle;
    private float muscleRatio;
    private float muscleStrength;

    //bone parts
    private Transform boneTrans;
    private Renderer boneRend;
    private Transform newBoneTrans;
    private Rigidbody2D newBoneRig;
    private Renderer newBoneRend;
    private Vector3 lastAngle;

    //key binding stuff
    private List<KeyCode> mapKeys;
    private int currentKey;
    private List<string> mapKeysText;

    // Start is called before the first frame update
    void Start()
    {
        this.hasBrain = false;
        this.drawmode = DrawMode.Brain;
        this.drawModeLabel.text = "Draw Object: Brain";
        this.boneTrans = Bone.GetComponent<Transform>();
        this.boneRend = Bone.GetComponent<Renderer>();
        this.newBone = null;
        this.boneRatio = boneTrans.localScale.x / boneRend.bounds.size.x;
        this.muscleRatio = this.Muscle.GetComponent<Transform>().localScale.x / this.Muscle.GetComponent<Renderer>().bounds.size.x;
        this.drawnParts = new List<GameObject>();
        this.drawingMuscle = false;
        this.newMuscle = null;
        if (isP1)
        {
            this.mapKeys = new List<KeyCode>() { KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow };
            this.mapKeysText = new List<string>() { "Up Arrow", "Left Arrow", "Down Arrow", "Right Arrow" };
        }
        else
        {
            this.mapKeys = new List<KeyCode>() { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
            this.mapKeysText = new List<string>() { "W", "A", "S", "D" };
        }
        this.currentKey = 0;

        this.muscleStrength = 2.5f;
    }

    void Update()
    {
        if (!this.isActive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (this.newBone != null)
            {
                Destroy(this.newBone);
                this.newBone = null;
            }
            if (this.newMuscle != null)
            {
                Destroy(this.newMuscle);
                this.newMuscle = null;
            }
            if (this.drawmode == DrawMode.Muscle)
            {
                this.drawmode = DrawMode.KeyBind;
                this.drawModeLabel.text = "Set Muscle Keys: " + this.mapKeysText[this.currentKey];
            }
            else if (this.drawmode == DrawMode.Brain)
            {
                this.drawmode = DrawMode.Bone;
                this.drawModeLabel.text = "Draw Object: Bone";
            }
            else if (this.drawmode == DrawMode.Bone)
            {
                this.drawmode = DrawMode.Muscle;
                this.drawModeLabel.text = "Draw Object: Muscle";
            }
            else if (this.drawmode == DrawMode.KeyBind)
            {
                if (this.hasBrain == false)
                {
                    this.drawmode = DrawMode.Brain;
                    this.drawModeLabel.text = "Draw Object: Brain";
                }
                else
                {
                    this.drawmode = DrawMode.Bone;
                    this.drawModeLabel.text = "Draw Object: Bone";
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            DeleteMode = true;
        } else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            DeleteMode = false;
        }

        if (Input.GetMouseButtonDown(0) && DeleteMode)
        {
            Vector3 mousePos = GameCam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);


            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero, Mathf.Infinity);
            RaycastHit2D hit = new RaycastHit2D();

            foreach (RaycastHit2D current in hits)
            {
                if (current.collider.gameObject.CompareTag("Muscle") || current.collider.gameObject.CompareTag("Bone"))
                {
                    hit = current;
                    break;
                }
                if (current.collider.gameObject.CompareTag("Brain"))
                {
                    this.hasBrain = false;
                    hit = current;
                    break;
                }
            }

            if (hit.collider != null)
            {
                Destroy(hit.collider.gameObject);
            }
            DeleteMode = false;
        }
        else if (newBone != null)
        {
            FollowMouseAnchor(this.newBone, this.startPoint, this.boneRatio);


            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (this.newBone.transform.localScale.y <= this.boneSizeMin)
                {
                    return;
                }

                Vector3 t = this.newBone.transform.localScale;
                this.newBone.transform.localScale = new Vector3(t.x, t.y - this.boneSizeMin, t.z);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (this.newBone.transform.localScale.y >= this.boneSizeMax)
                {
                    return;
                }

                Vector3 t = this.newBone.transform.localScale;
                this.newBone.transform.localScale = new Vector3(t.x, t.y + this.boneSizeMin, t.z);
            }
        }
        else if (this.newMuscle != null)
        {
            FollowMouseAnchor(this.newMuscle, this.startPoint, this.muscleRatio);

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (this.newMuscle.transform.localScale.y <= this.muscleStrengthMin)
                {
                    return;
                }
                
                Vector3 t = this.newMuscle.transform.localScale;
                this.newMuscle.transform.localScale = new Vector3(t.x, t.y - this.muscleStrengthMin, t.z);

                this.muscleStrength = t.y - this.muscleStrengthMin + 1.5f;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (this.newMuscle.transform.localScale.y >= this.muscleStrengthMax)
                {
                    return;
                }

                Vector3 t = this.newMuscle.transform.localScale;
                this.newMuscle.transform.localScale = new Vector3(t.x, t.y + this.muscleStrengthMin, t.z);

                this.muscleStrength = t.y + this.muscleStrengthMin + 1.5f;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f && this.drawmode == DrawMode.KeyBind)
        {
            this.currentKey++;
            if (this.currentKey >= this.mapKeys.Count)
            {
                this.currentKey = 0;
            }

            this.drawModeLabel.text = "Set Muscle Keys: " + this.mapKeysText[this.currentKey];
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && this.drawmode == DrawMode.KeyBind)
        {
            this.currentKey--;
            if (this.currentKey < 0)
            {
                this.currentKey = this.mapKeys.Count - 1;
            }

            this.drawModeLabel.text = "Set Muscle Keys: " + this.mapKeysText[this.currentKey];
        }
    }

    private void OnMouseDown()
    {
        if (DeleteMode || !this.isActive)
        {
            return;
        }

        //skelly bois rise up
        if (this.drawmode == DrawMode.Bone)
        {
            startPoint = GameCam.ScreenToWorldPoint(Input.mousePosition);

            Vector2 start = new Vector2(startPoint.x + (this.boneRend.bounds.size.x / 2), startPoint.y);
            this.newBone = Object.Instantiate(Bone);

            this.newBoneTrans = newBone.GetComponent<Transform>();
            this.newBoneRig = newBone.GetComponent<Rigidbody2D>();
            this.newBoneRend = newBone.GetComponent<Renderer>();

            this.newBoneTrans.position = start;
        }

        //bign boiy muscle tiem
        else if (this.drawmode == DrawMode.Muscle)
        {
            Vector3 mousePos = GameCam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D[] allHits = Physics2D.RaycastAll(mousePos2D, Vector2.zero, Mathf.Infinity);
            List<RaycastHit2D> hits = new List<RaycastHit2D>();

            foreach (RaycastHit2D hit in allHits)
            {
                if (hit.collider.gameObject.CompareTag("Bone") || hit.collider.gameObject.CompareTag("Brain"))
                {
                    hits.Add(hit);
                }
            }

            int len = hits.Count;

            if (this.drawingMuscle == false)
            {
                if (len > 0)
                {
                    GameObject firstHit = hits[0].collider.gameObject;
                    this.drawingMuscle = true;

                    Vector2 start = new Vector2(mousePos.x + (this.Muscle.GetComponent<Renderer>().bounds.size.x / 2), mousePos.y);
                    this.newMuscle = Object.Instantiate(Muscle);

                    this.drawnParts.Add(this.newMuscle);

                    this.muscleFirstBone = firstHit;
                    this.startPoint = mousePos;
                }
            }
            else
            {
                if (len > 0 && this.newMuscle != null)
                {
                    GameObject nextHit = hits[0].collider.gameObject;
                    SpringJoint2D newSpring = this.muscleFirstBone.AddComponent<SpringJoint2D>();
                    Vector2 oldPoint = new Vector2(this.startPoint.x, this.startPoint.y);
                    newSpring.anchor = this.muscleFirstBone.transform.InverseTransformPoint(oldPoint);
                    newSpring.connectedBody = nextHit.GetComponent<Rigidbody2D>();
                    newSpring.connectedAnchor = nextHit.transform.InverseTransformPoint(mousePos2D);
                    newSpring.autoConfigureDistance = false;
                    newSpring.enableCollision = true;

                    //joints so the muslcess obey you
                    GameObject joint1 = CreateBasicJointAtPoint(this.startPoint, this.muscleFirstBone);
                    GameObject joint2 = CreateBasicJointAtPoint(mousePos, nextHit);

                    newSpring.distance = Vector3.Distance(joint1.transform.position, joint2.transform.position);

                    MuscleBehavior nScript = this.newMuscle.GetComponent<MuscleBehavior>();
                    nScript.anchorJoint = joint1;
                    nScript.connectedAnchorJoint = joint2;
                    nScript.muscleScale = this.muscleRatio;
                    nScript.spring = newSpring;
                    nScript.SetSpringStrength(this.muscleStrength);

                    //keep a reference to the original joint holder
                    nextHit.GetComponent<DamageCheck>().connectedMuscles.Add(nScript);

                    this.muscleFirstBone = null;
                    this.drawingMuscle = false;
                    this.newMuscle = null;
                }
                else if (this.newMuscle != null)
                {
                    Destroy(this.newMuscle);
                    this.muscleFirstBone = null;
                    this.drawingMuscle = false;
                }
                else
                {
                    this.muscleFirstBone = null;
                    this.drawingMuscle = false;
                }
            }
        }

        //muscle groups
        else if (this.drawmode == DrawMode.KeyBind)
        {
            Vector3 mousePos = GameCam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D[] allHits = Physics2D.RaycastAll(mousePos2D, Vector2.zero, Mathf.Infinity);
            List<RaycastHit2D> hits = new List<RaycastHit2D>();

            foreach (RaycastHit2D hit in allHits)
            {
                if (hit.collider.gameObject.CompareTag("Muscle"))
                {
                    hits.Add(hit);
                }
            }

            int len = hits.Count;

            if (len > 0)
            {
                for (int i = 0; i < len; i++)
                {
                    MuscleBehavior nScript = hits[i].collider.gameObject.GetComponent<MuscleBehavior>();
                    nScript.SetFlexKey(this.mapKeys[this.currentKey]);
                }
            }
        }

        //oh yeah this is big brain time
        else if (this.drawmode == DrawMode.Brain && !this.hasBrain)
        {
            Vector3 mousePos = GameCam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            GameObject brain = Instantiate(this.Brain, mousePos2D, new Quaternion(0, 0, 0, 0));
            this.drawnParts.Add(brain);

            this.GameCam.GetComponent<WinCheck>().AssignBrain(this.isP1, brain);

            this.hasBrain = true;
        }
        
    }

    private void OnMouseUp()
    {
        if (DeleteMode || !this.isActive)
        {
            return;
        }

        //mr bones
        if (this.newBone != null && this.drawmode == DrawMode.Bone)
        {
            if (this.newBoneTrans.lossyScale.x == 0)
            {
                Destroy(this.newBone);
            }
            this.drawnParts.Add(this.newBone);
            this.newBoneTrans.position = new Vector3(this.newBoneTrans.position.x, this.newBoneTrans.position.y, 1);
            this.newBone.GetComponent<DamageCheck>().boneScale = this.boneRend.bounds.size.x;
            this.newBone = null;
        }

        
    }

    private void OnMouseOver()
    {
        if (!this.isActive)
        {
            return;
        }

        //joint shstuff
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = GameCam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D[] allHits = Physics2D.RaycastAll(mousePos2D, Vector2.zero, Mathf.Infinity);
            List<RaycastHit2D> hits = new List<RaycastHit2D>();

            foreach (RaycastHit2D hit in allHits)
            {
                if (hit.collider.gameObject.CompareTag("Bone") || hit.collider.gameObject.CompareTag("Brain"))
                {
                    hits.Add(hit);
                }
            }

            int len = hits.Count;

            if (len >= 2)
            {
                for (int i = 0; i < (len - 1); i++)
                {
                    

                    CreateJointAtPoint(mousePos, hits[i].collider.gameObject, hits[i + 1].collider.gameObject);
                }
            }
        }
    }

    //only give part-looking permissions to those who ask nicely
    public List<GameObject> GetParts()
    {
        return this.drawnParts;
    }

    //slap down a little jointy boi at the point and that parent be his dad
    //return him if you wanna reference him
    private GameObject CreateJointAtPoint(Vector3 origin, GameObject parent, GameObject other)
    {
        GameObject newJoint = Instantiate(Joint, origin, parent.transform.rotation, parent.transform);
        newJoint.transform.position = new Vector3(origin.x, origin.y, -1);
        Vector3 newScale = new Vector3(newJoint.transform.localScale.x / parent.transform.localScale.x, 1, 1);
        newJoint.transform.localScale = newScale;

        HingeJoint2D newHinge = parent.AddComponent<HingeJoint2D>();
        newHinge.connectedBody = other.GetComponent<Rigidbody2D>();
        newHinge.anchor = parent.transform.InverseTransformPoint(origin);

        JointData newDat = newJoint.GetComponent<JointData>();
        newDat.hinge = newHinge;

        other.transform.gameObject.GetComponent<DamageCheck>().connectedJoints.Add(newDat);

        return newJoint;
    }

    private GameObject CreateBasicJointAtPoint(Vector3 origin, GameObject parent)
    {
        GameObject newJoint = Instantiate(Joint, origin, parent.transform.rotation, parent.transform);
        newJoint.transform.position = new Vector3(origin.x, origin.y, -1);
        Vector3 newScale = new Vector3(newJoint.transform.localScale.x / parent.transform.localScale.x, 1, 1);
        newJoint.transform.localScale = newScale;

        return newJoint;
    }

    //left edge on the anchor, right edge on the mouse position. requires a ratio for the the renderer distance and the scale beforehand
    //  because that bad boi could be zero or negative and fractions hate that shite
    private void FollowMouseAnchor(GameObject follower, Vector3 anchor, float rendererScaleRatio)
    {
        Vector3 mousePos = this.GameCam.ScreenToWorldPoint(Input.mousePosition);
        Transform followerTrans = follower.GetComponent<Transform>();

        Vector2 direction = mousePos - followerTrans.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        followerTrans.rotation = rotation;

        float mouseDisX = Vector3.Distance(mousePos, anchor);
        float newScale = mouseDisX * rendererScaleRatio;
        Vector3 modScale = followerTrans.localScale;
        modScale.x = newScale;
        followerTrans.localScale = modScale;

        Vector2 newPos = (anchor + mousePos) * 0.5f;
        followerTrans.position = newPos;
    }
}
