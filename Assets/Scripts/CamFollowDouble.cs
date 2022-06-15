using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowDouble : MonoBehaviour
{
    public float camLerpT;
    public float camSpeed;

    private Transform player1;
    private Transform player2;
    private float minZ;

    public void BeginGame(Transform player1Trans, Transform player2Trans) {
        try{
            player1 = player1Trans.GetComponentInChildren<Brain>().transform;
            player2 = player2Trans.GetComponentInChildren<Brain>().transform;
        } catch {
            Destroy(this);
        }

        minZ = transform.position.z;
        
    }

    public void Update() {
        //move to the midpoint between them

        //try to zoom out if either of them is close to off-screen

        //otherwise, try to zoom in if your Z is not the min Z

        // if (player1 != null) {
        //     Vector3 approachPos = Vector3.Lerp(transform.position, player.position, camLerpT*Time.deltaTime);
        //     transform.position = new Vector3(approachPos.x, approachPos.y, transform.position.z);
        // }
    }
}
