using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawParts : MonoBehaviour
{
    public GameObject bonePref;
    public GameObject joint;
    public GameObject muscle;
    public GameObject brain;
    public Camera gameCam;
    public Text drawModeLabel;
    public float muscleStrengthMax;
    public float muscleStrengthMin;
    public float boneSizeMin;
    public float boneSizeMax;
    public bool isP1;
    public bool isActive;
    public List<KeyCode> controlKeys;

    private bool _deleteMode = false;
    private Vector3 _startPoint;
    private GameObject _newBone;
    private float _boneRatio;
    private List<GameObject> _drawnParts;
    private enum DrawMode
    {
        Brain = 0,
        Bone = 1,
        Muscle = 2,
        KeyBind = 3
    };

    private Dictionary<DrawMode, string> _drawText;
    private DrawMode _drawMode;
    private bool _hasBrain;
    private Part _drawingPart;

    //muscle stuff
    private bool _drawingMuscle;
    private GameObject _muscleFirstBone;
    private GameObject _newMuscle;
    private float _muscleRatio;
    private float _muscleStrength;

    //bone parts
    private Transform _boneTrans;
    private Renderer _boneRend;
    private Transform _newBoneTrans;
    
    //key binding stuff
    private int _currentKey;

    void Start()
    {
        _hasBrain = false;
        _drawText = new Dictionary<DrawMode, string>
        {
            {DrawMode.Brain, "Draw Object: Brain"},
            {DrawMode.Bone, "Draw Object: Bone"},
            {DrawMode.Muscle, "Draw Object: Muscle"},
            {DrawMode.KeyBind, "Set Muscle Keys: " + controlKeys[_currentKey]}
        };
        SetDrawing(DrawMode.Brain);
        
        //fix line
        
        _newBone = null;
        _boneRatio = bonePref.GetComponent<Transform>().localScale.x / bonePref.GetComponent<Renderer>().bounds.size.x;
        this._muscleRatio = this.muscle.GetComponent<Transform>().localScale.x / this.muscle.GetComponent<Renderer>().bounds.size.x;
        this._drawnParts = new List<GameObject>();
        this._drawingMuscle = false;
        this._newMuscle = null;
        
        this._currentKey = 0;

        this._muscleStrength = 2.5f;
    }

    private void SetDrawing(DrawMode mode)
    {
        _drawMode = mode;
        drawModeLabel.text = _drawText[mode];
    }

    void Update()
    {
        if (!this.isActive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (this._newBone != null)
            {
                Destroy(this._newBone);
                this._newBone = null;
            }
            if (this._newMuscle != null)
            {
                Destroy(this._newMuscle);
                this._newMuscle = null;
            }
            if (this._drawMode == DrawMode.Muscle)
            {
                this._drawMode = DrawMode.KeyBind;
                this.drawModeLabel.text = "Set Muscle Keys: " + controlKeys[this._currentKey];
            }
            else if (this._drawMode == DrawMode.Brain)
            {
                this._drawMode = DrawMode.Bone;
                this.drawModeLabel.text = "Draw Object: Bone";
            }
            else if (this._drawMode == DrawMode.Bone)
            {
                this._drawMode = DrawMode.Muscle;
                this.drawModeLabel.text = "Draw Object: Muscle";
            }
            else if (this._drawMode == DrawMode.KeyBind)
            {
                if (this._hasBrain == false)
                {
                    this._drawMode = DrawMode.Brain;
                    this.drawModeLabel.text = "Draw Object: Brain";
                }
                else
                {
                    this._drawMode = DrawMode.Bone;
                    this.drawModeLabel.text = "Draw Object: Bone";
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _deleteMode = true;
        } else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _deleteMode = false;
        }

        if (Input.GetMouseButtonDown(0) && _deleteMode)
        {
            Vector3 mousePos = gameCam.ScreenToWorldPoint(Input.mousePosition);
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
                    this._hasBrain = false;
                    hit = current;
                    break;
                }
            }

            if (hit.collider != null)
            {
                Destroy(hit.collider.gameObject);
            }
            _deleteMode = false;
        }
        else if (_newBone != null)
        {
            FollowMouseAnchor(this._newBone, this._startPoint, this._boneRatio);


            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (this._newBone.transform.localScale.y <= this.boneSizeMin)
                {
                    return;
                }

                Vector3 t = this._newBone.transform.localScale;
                this._newBone.transform.localScale = new Vector3(t.x, t.y - this.boneSizeMin, t.z);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (this._newBone.transform.localScale.y >= this.boneSizeMax)
                {
                    return;
                }

                Vector3 t = this._newBone.transform.localScale;
                this._newBone.transform.localScale = new Vector3(t.x, t.y + this.boneSizeMin, t.z);
            }
        }
        else if (this._newMuscle != null)
        {
            FollowMouseAnchor(this._newMuscle, this._startPoint, this._muscleRatio);

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (this._newMuscle.transform.localScale.y <= this.muscleStrengthMin)
                {
                    return;
                }
                
                Vector3 t = this._newMuscle.transform.localScale;
                this._newMuscle.transform.localScale = new Vector3(t.x, t.y - this.muscleStrengthMin, t.z);

                this._muscleStrength = t.y - this.muscleStrengthMin + 1.5f;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (this._newMuscle.transform.localScale.y >= this.muscleStrengthMax)
                {
                    return;
                }

                Vector3 t = this._newMuscle.transform.localScale;
                this._newMuscle.transform.localScale = new Vector3(t.x, t.y + this.muscleStrengthMin, t.z);

                this._muscleStrength = t.y + this.muscleStrengthMin + 1.5f;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f && this._drawMode == DrawMode.KeyBind)
        {
            this._currentKey++;
            if (this._currentKey >= controlKeys.Count)
            {
                this._currentKey = 0;
            }

            this.drawModeLabel.text = "Set Muscle Keys: " + controlKeys[this._currentKey];
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && this._drawMode == DrawMode.KeyBind)
        {
            this._currentKey--;
            if (this._currentKey < 0)
            {
                this._currentKey = controlKeys.Count - 1;
            }

            this.drawModeLabel.text = "Set Muscle Keys: " + controlKeys[this._currentKey];
        }
    }

    private void OnMouseDown()
    {
        if (_deleteMode || !this.isActive)
        {
            return;
        }

        //skelly bois rise up
        if (this._drawMode == DrawMode.Bone)
        {
            _startPoint = gameCam.ScreenToWorldPoint(Input.mousePosition);

            Vector2 start = new Vector2(_startPoint.x + (this._boneRend.bounds.size.x / 2), _startPoint.y);
            this._newBone = Object.Instantiate(bonePref);

            this._newBoneTrans = _newBone.GetComponent<Transform>();

            this._newBoneTrans.position = start;
        }

        //bign boiy muscle tiem
        else if (this._drawMode == DrawMode.Muscle)
        {
            Vector3 mousePos = gameCam.ScreenToWorldPoint(Input.mousePosition);
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

            if (this._drawingMuscle == false)
            {
                if (len > 0)
                {
                    GameObject firstHit = hits[0].collider.gameObject;
                    this._drawingMuscle = true;

                    Vector2 start = new Vector2(mousePos.x + (this.muscle.GetComponent<Renderer>().bounds.size.x / 2), mousePos.y);
                    this._newMuscle = Object.Instantiate(muscle);

                    this._drawnParts.Add(this._newMuscle);

                    this._muscleFirstBone = firstHit;
                    this._startPoint = mousePos;
                }
            }
            else
            {
                if (len > 0 && this._newMuscle != null)
                {
                    GameObject nextHit = hits[0].collider.gameObject;
                    SpringJoint2D newSpring = this._muscleFirstBone.AddComponent<SpringJoint2D>();
                    Vector2 oldPoint = new Vector2(this._startPoint.x, this._startPoint.y);
                    newSpring.anchor = this._muscleFirstBone.transform.InverseTransformPoint(oldPoint);
                    newSpring.connectedBody = nextHit.GetComponent<Rigidbody2D>();
                    newSpring.connectedAnchor = nextHit.transform.InverseTransformPoint(mousePos2D);
                    newSpring.autoConfigureDistance = false;
                    newSpring.enableCollision = true;

                    //joints so the muslcess obey you
                    GameObject joint1 = CreateBasicJointAtPoint(this._startPoint, this._muscleFirstBone);
                    GameObject joint2 = CreateBasicJointAtPoint(mousePos, nextHit);

                    newSpring.distance = Vector3.Distance(joint1.transform.position, joint2.transform.position);

                    MuscleBehavior nScript = this._newMuscle.GetComponent<MuscleBehavior>();
                    nScript.anchorJoint = joint1;
                    nScript.connectedAnchorJoint = joint2;
                    nScript.muscleScale = this._muscleRatio;
                    nScript.spring = newSpring;
                    nScript.SetSpringStrength(this._muscleStrength);

                    //keep a reference to the original joint holder
                    nextHit.GetComponent<DamageCheck>().connectedMuscles.Add(nScript);

                    this._muscleFirstBone = null;
                    this._drawingMuscle = false;
                    this._newMuscle = null;
                }
                else if (this._newMuscle != null)
                {
                    Destroy(this._newMuscle);
                    this._muscleFirstBone = null;
                    this._drawingMuscle = false;
                }
                else
                {
                    this._muscleFirstBone = null;
                    this._drawingMuscle = false;
                }
            }
        }

        //muscle groups
        else if (this._drawMode == DrawMode.KeyBind)
        {
            Vector3 mousePos = gameCam.ScreenToWorldPoint(Input.mousePosition);
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
                    nScript.SetFlexKey(controlKeys[this._currentKey]);
                }
            }
        }

        //oh yeah this is big brain time
        else if (this._drawMode == DrawMode.Brain && !this._hasBrain)
        {
            Vector3 mousePos = gameCam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            GameObject brain = Instantiate(this.brain, mousePos2D, new Quaternion(0, 0, 0, 0));
            this._drawnParts.Add(brain);

            this.gameCam.GetComponent<WinCheck>().AssignBrain(this.isP1, brain);

            this._hasBrain = true;
        }
        
    }

    private void OnMouseUp()
    {
        if (_deleteMode || !this.isActive)
        {
            return;
        }

        //mr bones
        if (this._newBone != null && this._drawMode == DrawMode.Bone)
        {
            if (this._newBoneTrans.lossyScale.x == 0)
            {
                Destroy(this._newBone);
            }
            this._drawnParts.Add(this._newBone);
            this._newBoneTrans.position = new Vector3(this._newBoneTrans.position.x, this._newBoneTrans.position.y, 1);
            this._newBone.GetComponent<DamageCheck>().boneScale = this._boneRend.bounds.size.x;
            this._newBone = null;
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
            Vector3 mousePos = gameCam.ScreenToWorldPoint(Input.mousePosition);
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
        return this._drawnParts;
    }

    //slap down a little jointy boi at the point and that parent be his dad
    //return him if you wanna reference him
    private GameObject CreateJointAtPoint(Vector3 origin, GameObject parent, GameObject other)
    {
        GameObject newJoint = Instantiate(joint, origin, parent.transform.rotation, parent.transform);
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
        GameObject newJoint = Instantiate(joint, origin, parent.transform.rotation, parent.transform);
        newJoint.transform.position = new Vector3(origin.x, origin.y, -1);
        Vector3 newScale = new Vector3(newJoint.transform.localScale.x / parent.transform.localScale.x, 1, 1);
        newJoint.transform.localScale = newScale;

        return newJoint;
    }

    //left edge on the anchor, right edge on the mouse position. requires a ratio for the the renderer distance and the scale beforehand
    //  because that bad boi could be zero or negative and fractions hate that shite
    private void FollowMouseAnchor(GameObject follower, Vector3 anchor, float rendererScaleRatio)
    {
        Vector3 mousePos = this.gameCam.ScreenToWorldPoint(Input.mousePosition);
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
