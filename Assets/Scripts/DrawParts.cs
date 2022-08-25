using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;


public class DrawParts : MonoBehaviour
{
    public GameObject bonePref;
    public GameObject jointPref;
    public GameObject musclePref;
    public GameObject brainPref;
    public GameObject playerPref;
    public Text drawModeLabel;
    public bool isP1;
    public bool isActive;
    public List<KeyCode> controlKeys;
    public Dictionary<DrawControl, KeyCode> drawKeys;
    public bool hasBrain;
    //public bool loadSavedPlayer = false;

    public enum DrawControl
    {
        SwitchPart, //space
        DeleteMode, //lshift
        PlacePart, //mouse0
        PlaceJoint, //mouse1
    }

    private bool _deleteMode;
    private List<GameObject> _drawnParts;
    private GameObject _player;

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

    public GameObject GetPlayer()
    {
        return _player;
    }

    void Start()
    {
        //remove these later, for the initial population of text file
        if (isP1)
        {
            controlKeys = new List<KeyCode>()
            {
                KeyCode.UpArrow,
                KeyCode.DownArrow,
                KeyCode.LeftArrow,
                KeyCode.RightArrow,
                KeyCode.Backslash,
                KeyCode.RightShift
            };
        }
        else
        {
            controlKeys = new List<KeyCode>()
            {
                KeyCode.W,
                KeyCode.S,
                KeyCode.A,
                KeyCode.D,
                KeyCode.Tab,
                KeyCode.LeftShift
            };
        }

        drawKeys = new Dictionary<DrawParts.DrawControl, KeyCode>()
        {
            { DrawParts.DrawControl.DeleteMode, KeyCode.LeftShift },
            { DrawParts.DrawControl.PlaceJoint, KeyCode.Mouse1 },
            { DrawParts.DrawControl.PlacePart, KeyCode.Mouse0 },
            { DrawParts.DrawControl.SwitchPart, KeyCode.Space }
        };


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

    public void AttemptLoad()
    {
        try
        {
            _player = Instantiate<GameObject>(PreviousHomonculus.instance.go, gameObject.transform.position, Quaternion.identity);
            
            _player.SetActive(true);

            foreach (Transform child in _player.GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.CompareTag("Untagged"))
                {
                    continue;
                }

                if (child.gameObject.CompareTag("Brain"))
                {
                    SetBrain(child.gameObject);
                }

                _drawnParts.Add(child.gameObject);
            }
        }
        catch
        {
            _player = Instantiate<GameObject>(playerPref, gameObject.transform.position, Quaternion.identity);
        }
    }

    public void SavePlayer()
    {
        if (PreviousHomonculus.instance.go != null) {
            Destroy(PreviousHomonculus.instance.go);
        }
        
        GameObject savedGO = Instantiate<GameObject>(_player);
        savedGO.SetActive(false);
        DontDestroyOnLoad(savedGO);
        PreviousHomonculus.instance.go = savedGO;
    }

    private void SetDrawing(DrawMode mode)
    {
        _drawMode = mode;
        drawModeLabel.text = _drawText[mode];
        _drawingPartPref = _drawPartMap[mode];
    }

    public void RemovePart(Part p)
    {
        Debug.Log("removing part");
        _drawnParts.Remove(p.gameObject);
        _player.GetComponent<Homonculus>().ReportDestroyedPart(p);
        Destroy(p.gameObject);
    }

    void Update()
    {
        if (!isActive)
        {
            return;
        }

        if (PauseManager.Instance.IsPaused)
        {
            return;
        }

        if (Input.GetKeyDown(drawKeys[DrawControl.SwitchPart]))
        {
            if (_drawingPart != null)
            {
                RemovePart(_drawingPart);
                _drawingPart = null;
            }

            _drawMode++;
            if ((int)_drawMode > _drawPartMap.Keys.Count - 1)
            {
                _drawMode = 0;
            }

            SetDrawing(_drawMode);
        }

        if (Input.GetKeyDown(drawKeys[DrawControl.DeleteMode]))
        {
            _deleteMode = true;
        }
        else if (Input.GetKeyUp(drawKeys[DrawControl.DeleteMode]))
        {
            _deleteMode = false;
        }

        if (Input.GetKeyDown(drawKeys[DrawControl.PlacePart]))
        {
            if (_deleteMode)
            {
                DeleteParts();
            }
            else
            {
                MouseClickHandler();
            }
        }
        else if (Input.GetKeyUp(drawKeys[DrawControl.PlacePart]))
        {
            //maybe complete draw stroke regardless?
            if (!_deleteMode)
            {
                CompleteDrawStroke();
            }
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
                if (_currentKey > controlKeys.Count - 1)
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

        foreach (RaycastHit2D current in hits)
        {
            if (current.collider.gameObject.CompareTag("DrawZone"))
            {
                continue;
            }

            if (current.collider.gameObject.CompareTag("Brain"))
            {
                hasBrain = false;
            }

            RemovePart(current.collider.gameObject.GetComponent<Part>());
        }
    }

    private void MouseClickHandler()
    {
        if (_deleteMode || !isActive)
        {
            return;
        }

        if (_drawMode.Equals(DrawMode.KeyBind))
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

        _drawingPart = Instantiate(_drawingPartPref, _player.transform).GetComponent<Part>();
        _drawnParts.Add(_drawingPart.gameObject);

        _drawingPart.StartDraw(this);
    }

    public void SetBrain(GameObject brain)
    {
        WinCheck winChecker = Camera.main.GetComponent<WinCheck>();

        if (winChecker != null)
        {
            Camera.main.GetComponent<WinCheck>().AssignBrain(isP1, brain);
        }

        _player.GetComponent<Homonculus>().ReportBrain(brain.GetComponent<Part>());

        hasBrain = true;
    }

    private void CompleteDrawStroke()
    {
        if (_deleteMode || !isActive)
        {
            return;
        }

        if (_drawingPart != null)
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
        if (Input.GetKeyDown(drawKeys[DrawControl.PlaceJoint]))
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
                GameObject joint = null;

                for (int i = 0; i < (len - 1); i++)
                {
                    joint = CreateJointAtPoint(mousePos, hits[i].collider.gameObject, hits[i + 1].collider.gameObject);
                }

                joint.GetComponent<MyJoint>().FinishDraw(this);
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
        GameObject newJointGo = CreateBasicJointAtPoint(origin, parent, other);

        HingeJoint2D newHinge = parent.AddComponent<HingeJoint2D>();
        newHinge.connectedBody = other.GetComponent<Rigidbody2D>();
        newHinge.anchor = parent.transform.InverseTransformPoint(origin);

        MyJoint newMyJoint = newJointGo.GetComponent<MyJoint>();
        newMyJoint.hinge = newHinge;

        return newJointGo;
    }

    public GameObject CreateBasicJointAtPoint(Vector3 origin, GameObject parent, GameObject other)
    {

        GameObject newJointGo = Instantiate(jointPref, origin, parent.transform.rotation, parent.transform);
        newJointGo.transform.position = new Vector3(origin.x, origin.y, -3);
        Vector3 newScale = new Vector3(1 / parent.transform.localScale.x, 1 / parent.transform.localScale.y, 1);
        newJointGo.transform.localScale = newScale;

        MyJoint newMyJoint = newJointGo.GetComponent<MyJoint>();

        // other.transform.gameObject.GetComponent<Part>().connectedJoints.Add(newMyJoint);
        // parent.transform.gameObject.GetComponent<Part>().connectedJoints.Add(newMyJoint);

        // newMyJoint.AddConnection(parent.GetComponent<Part>(), other.GetComponent<Part>());

        newMyJoint.connectedPart = other.GetComponent<Part>();
        _player.GetComponent<Homonculus>().ReportConnection(parent.GetComponent<Part>(), other.GetComponent<Part>());

        return newJointGo;
    }
}