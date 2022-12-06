using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Listable : MonoBehaviour
{
    protected int currentIndex;

    public virtual void Start() {
        currentIndex = 0;
    }

    public abstract void GoNext();
    
    public abstract void GoPrevious();
}
