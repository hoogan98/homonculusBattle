using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{

    private Animation startAnim;

    void Start()
    {
        startAnim = GetComponent<Animation>();
        startAnim.wrapMode = WrapMode.Once;
    }

    public void Suicide() {
        Destroy(gameObject);
    }
}
