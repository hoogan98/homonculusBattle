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
    public Text drawModeLabel;
    public bool isP1;
    public bool isActive;
    public List<KeyCode> controlKeys;
    public bool hasBrain;

    private bool _deleteMode;
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
    private GameObject _drawingPartPref;
    private Part _drawingPart;

    //key binding stuff
    private int _currentKey;

    void Start()
    {
        hasBrain = false;
        _drawText = new Dictionary<DrawMode, string>
        {
            { DrawMode.Brain, "Draw Object: Brain" },
            { DrawMode.Bone, "Draw Object: Bone" },
            { DrawMode.Muscle, "Draw Object: Muscle" },
            { DrawMode.KeyBind, "Set Muscle Keys: " + controlKeys[_currentKey] }
        };
        _drawPartMap = new Dictionary<DrawMode, GameObject>
        {
            { DrawMode.Brain, brainPref },
            { DrawMode.Bone, bonePref },
            { DrawMode.Muscle, musclePref },
            { DrawMode.KeyBind, null }
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
        Destroy(p.gameObject);
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
            if ((int)_drawMode > _drawPartMap.Keys.Count)
            {
                _drawMode = 0;
            }

            SetDrawing(_drawMode);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _deleteMode = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
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
        }
        else if (_drawMode == DrawMode.KeyBind)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                _currentKey++;
                if (_currentKey >= controlKeys.Count - 1)
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
                hasBrain = false;
            }

            _drawnParts.Remove(current.collider.gameObject);

            Destroy(hit.collider.gameObject);
        }
    }

    private void OnMouseDown()
    {
        if (_deleteMode || !isActive)
        {
            return;
        }

        if (_drawMode == DrawMode.KeyBind)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D[] allHits = Physics2D.RaycastAll(mousePos, Vector2.zero, Mathf.Infinity);
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
                    Muscle nScript = hits[i].collider.gameObject.GetComponent<Muscle>();
                    nScript.SetFlexKey(controlKeys[_currentKey]);
                }
            }

            return;
        }

        _drawingPart = Instantiate(_drawingPartPref).GetComponent<Part>();
        _drawnParts.Add(_drawingPart.gameObject);
        float ratio = _drawingPartPref.GetComponent<Transform>().localScale.x /
                      _drawingPartPref.GetComponent<Renderer>().bounds.size.x;
        _drawingPart.StartDraw(this, ratio);
    }

    public void SetBrain(GameObject brain)
    {
        Camera.main.GetComponent<WinCheck>().AssignBrain(isP1, brain);

        hasBrain = true;
    }

    private void OnMouseUp()
    {
        if (_deleteMode || !isActive)
        {
            return;
        }

        if (_drawingPart != null && _drawMode == DrawMode.Bone)
        {
            _drawingPart.FinishDraw(this);
        }
    }

    public void EndDraw()
    {
        _drawingPart = null;
    }

    private void OnMouseOver()
    {
        if (!isActive)
        {
            return;
        }

        //joint shstuff
        if (Input.GetMouseButtonDown(1))
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

            if (len >= 2)
            {
                for (int i = 0; i < (len - 1); i++)
                {
                    CreateJointAtPoint(mousePos, hits[i].collider.gameObject, hits[i + 1].collider.gameObject);
                }
            }
        }
    }

    public List<GameObject> GetParts()
    {
        return _drawnParts;
    }

    //slap down a little jointy boi at the point and that parent be his dad
    //return him if you wanna reference him
    public GameObject CreateJointAtPoint(Vector3 origin, GameObject parent, GameObject other)
    {
        GameObject newJointGo = CreateBasicJointAtPoint(origin, parent);

        HingeJoint2D newHinge = parent.AddComponent<HingeJoint2D>();
        newHinge.connectedBody = other.GetComponent<Rigidbody2D>();
        newHinge.anchor = parent.transform.InverseTransformPoint(origin);

        MyJoint newMyJoint = newJointGo.GetComponent<MyJoint>();
        newMyJoint.hinge = newHinge;

        other.transform.gameObject.GetComponent<Part>().connectedJoints.Add(newMyJoint);

        return newJointGo;
    }

    public GameObject CreateBasicJointAtPoint(Vector3 origin, GameObject parent)
    {
        GameObject newJointGo = Instantiate(jointPref, origin, parent.transform.rotation, parent.transform);
        newJointGo.transform.position = new Vector3(origin.x, origin.y, -1);
        Vector3 newScale = new Vector3(newJointGo.transform.localScale.x / parent.transform.localScale.x, 1, 1);
        newJointGo.transform.localScale = newScale;

        return newJointGo;
    }
}