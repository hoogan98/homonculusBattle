using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClear : MonoBehaviour
{
    public List<GameObject> objects;

    public void Clear() {
        foreach (GameObject del in objects) {
            Destroy(del);
        }
        
    }
}
