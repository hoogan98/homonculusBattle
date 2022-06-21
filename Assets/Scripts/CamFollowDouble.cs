using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowDouble : MonoBehaviour
{
    public float camLerpT;
    public float camZoomSpeed;

    private Transform player1;
    private Transform player2;
    private float maxZ;
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

        maxZ = transform.position.z;
        
        p1Visible = true;
        p2Visible = true;

    }

    public void SetVisibility(bool isP1, bool isVisible)
    {
        if (isP1)
        {
            p1Visible = isVisible;
        }
        else
        {
            p2Visible = isVisible;
        }
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

                float newZ = transform.position.z;

                //try to zoom out if either of them is close to off-screen
                if (!p1Visible || !p2Visible)
                {
                    newZ -= camZoomSpeed * Time.deltaTime;
                }
                else if (newZ < maxZ)
                {
                    //otherwise, try to zoom in if your Z is not the max Z
                    newZ += camZoomSpeed * Time.deltaTime;
                }

                if (newZ > maxZ)
                {
                    newZ = maxZ;
                }

                transform.position = new Vector3(midPos.x, midPos.y, newZ);

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


        // if (player1 != null) {
        //     Vector3 approachPos = Vector3.Lerp(transform.position, player.position, camLerpT*Time.deltaTime);
        //     transform.position = new Vector3(approachPos.x, approachPos.y, transform.position.z);
        // }
    }
}
