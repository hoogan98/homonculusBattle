using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowSingle : MonoBehaviour
{
    public float camLerpT;
    
    private Transform player;

    public void BeginGame(Transform playerTrans) {
        try{
            player = playerTrans.GetComponentInChildren<Brain>().transform;
        } catch {
            Destroy(this);
        }
        
    }

    public void Update() {
        if (player != null) {
            Vector3 approachPos = Vector3.Lerp(transform.position, player.position, camLerpT*Time.deltaTime);
            transform.position = new Vector3(approachPos.x, approachPos.y, transform.position.z);
        }
    }
}
