using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOList : Listable
{
    public GameObject[] objects;

    public override void GoNext()
    {
        if (currentIndex + 1 >= objects.Length) {
            currentIndex = 0;
        } else {
            currentIndex++;
        }

        RegenerateChild();
    }

    public override void GoPrevious()
    {
        if (currentIndex - 1 <= 0) {
            currentIndex = objects.Length - 1;
        } else {
            currentIndex--;
        }

        RegenerateChild();
    }

    private void RegenerateChild() {
        Destroy(transform.GetChild(0).gameObject);
        Instantiate<GameObject>(objects[currentIndex], this.transform);
    }
}
