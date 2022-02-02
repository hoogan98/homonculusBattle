using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawParts : MonoBehaviour
{
    public GameObject bonePref;
    public GameObject jointPref;
    public GameObject musclePref;
    public GameObject brainPref;
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
    private Dictionary<DrawMode, GameObject> _drawPartMap;
    private DrawMode _drawMode;
    private bool _hasBrain;
    private GameObject _drawingPartPref;
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
        _drawPartMap = new Dictionary<DrawMode, GameObject>
        {
            {DrawMode.Brain, brainPref},
            {DrawMode.Bone, bonePref},
            {DrawMode.Muscle, musclePref},
            {DrawMode.KeyBind, null}
        };
        SetDrawing(DrawMode.Brain);
        
        _drawnParts = new List<GameObject>();
        
        _currentKey = 0;
    }

    private void SetDrawing(DrawMode mode)
    {
        _drawMode = mode;
        drawModeLabel.text = _drawText[mode];
        _drawingPartPref = _drawPartMap[mode];
    }

    public void RemovePart(Part p)
    {
        _drawnParts.Remove(p.gameObject);
    }

    void Update()
    {
        if (!isActive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_drawingPart != null)
            {
                _drawnParts.Remove(_drawingPart.gameObject);
                Destroy(_drawingPart.gameObject);
                _drawingPart = null;
            }
            
            _drawMode++;
            if (_drawMode.CompareTo(_drawPartMap.Keys.Count) > 0)
            {
                _drawMode = 0;
            }
            
            SetDrawing(_drawMode);
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
            DeleteParts();
        }

        if (_drawingPart != null)
        {
            _drawingPart.DrawingBehavior();
        } else if (_drawMode == DrawMode.KeyBind)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                _currentKey++;
                if (_currentKey >= controlKeys.Count)
                {
                    _currentKey = 0;
                }

                drawModeLabel.text = "Set Muscle Keys: " + controlKeys[_currentKey];
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                _currentKey--;
                if (_currentKey < 0)
                {
                    _currentKey = controlKeys.Count - 1;
                }

                drawModeLabel.text = "Set Muscle Keys: " + controlKeys[_currentKey];
            }
        }
    }

    private void DeleteParts()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero, Mathf.Infinity);
        RaycastHit2D hit = new RaycastHit2D();

        foreach (RaycastHit2D current in hits)
        {
            if (current.collider.gameObject.CompareTag("Brain"))
            {
                _hasBrain = false;
            }

            _drawnParts.Remove(current.collider.gameObject);
                
            Destroy(hit.collider.gameObject);
        }
    }

    private void OnMouseDown()
    {
        if (_deleteMode || !this.isActive)
        {
            return;
        }

        _drawingPart = Instantiate(_drawingPartPref).GetComponent<Part>();
        _drawingPart.StartDraw(this);
        _drawnParts.Add(_drawingPart.gameObject);
        
        //fix line
        //be sure to delete musclebehavior file

        //muscle groups
         if (this._drawMode == DrawMode.KeyBind)
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

            GameObject brain = Instantiate(this.brainPref, mousePos2D, new Quaternion(0, 0, 0, 0));
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
    public GameObject CreateJointAtPoint(Vector3 origin, GameObject parent, GameObject other)
    {
        GameObject newJoint = Instantiate(jointPref, origin, parent.transform.rotation, parent.transform);
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

    public GameObject CreateBasicJointAtPoint(Vector3 origin, GameObject parent)
    {
        GameObject newJoint = Instantiate(jointPref, origin, parent.transform.rotation, parent.transform);
        newJoint.transform.position = new Vector3(origin.x, origin.y, -1);
        Vector3 newScale = new Vector3(newJoint.transform.localScale.x / parent.transform.localScale.x, 1, 1);
        newJoint.transform.localScale = newScale;

        return newJoint;
    }
}
