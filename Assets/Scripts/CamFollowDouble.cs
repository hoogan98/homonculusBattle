using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowDouble : MonoBehaviour
{
    public float camLerpT;
    public float camZoomSpeed;
    public float maxPlayerDistCoeff;
    public float minPlayerDistCoeff;

    private Transform player1;
    private Transform player2;
    private float minSize;
    private Camera cam;
    private bool p1Visible;
    private bool p2Visible;

    public void BeginGame(Transform player1Trans, Transform player2Trans)
    {
        try
        {
            player1 = player1Trans.GetComponentInChildren<Brain>().transform;
            player2 = player2Trans.GetComponentInChildren<Brain>().transform;
        }
        catch
        {
            Destroy(this);
        }

        cam = GetComponent<Camera>();

        minSize = cam.orthographicSize;
        
        p1Visible = true;
        p2Visible = true;

    }

    private float CamWidth() {
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        return width;
    }

    private bool CamTooSmall() {
        return Vector3.Distance(player1.position, player2.position) > (CamWidth() * maxPlayerDistCoeff);
    }

    private bool CamTooBig() {
        return Vector3.Distance(player1.position, player2.position) < (CamWidth() * minPlayerDistCoeff);
    }

    public void Update()
    {
        if (player1 != null)
        {
            if (player2 != null)
            {
                //move to the midpoint between them
                Vector3 midPos = Vector3.Lerp(transform.position, (player1.position + player2.position) / 2,
                camLerpT * Time.deltaTime);

                float newSize = cam.orthographicSize;

                //try to zoom out if either of them is close to off-screen
                if (CamTooSmall())
                {
                    newSize += camZoomSpeed * Time.deltaTime;
                }
                else if (CamTooBig())
                {
                    //otherwise, try to zoom in
                    newSize -= camZoomSpeed * Time.deltaTime;
                }

                if (newSize < minSize)
                {
                    newSize = minSize;
                }

                transform.position = new Vector3(midPos.x, midPos.y, transform.position.z);
                cam.orthographicSize = newSize;

                return;
            }

            Vector3 approachPos = Vector3.Lerp(transform.position, player1.position, camLerpT * Time.deltaTime);
            transform.position = new Vector3(approachPos.x, approachPos.y, transform.position.z);
        }
        else if (player2 != null)
        {
            Vector3 approachPos = Vector3.Lerp(transform.position, player2.position, camLerpT * Time.deltaTime);
            transform.position = new Vector3(approachPos.x, approachPos.y, transform.position.z);
        }
    }
}
