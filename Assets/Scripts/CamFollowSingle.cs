using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowSingle : CamFollow
{
    public float camLerpT;
    

    public void BeginGame(Transform playerTrans) {
        try{
            player1 = playerTrans.GetComponentInChildren<Brain>().transform;
        } catch {
            Destroy(this);
        }
        
    }

    public void Update() {
        if (player1 != null) {
            Vector3 approachPos = Vector3.Lerp(transform.position, player1.position, camLerpT*Time.deltaTime);
            transform.position = new Vector3(approachPos.x, approachPos.y, transform.position.z);
        }
    }
}
