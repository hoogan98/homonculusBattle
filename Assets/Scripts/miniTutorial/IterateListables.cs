using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IterateListables : MonoBehaviour
{
    public Listable[] listables;

    public void MoveNext() {
        foreach(Listable listable in listables) {
            listable.GoNext();
        }
    }

    public void MovePrevious() {
        foreach(Listable listable in listables) {
            listable.GoPrevious();
        }
    }
}
