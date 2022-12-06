using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageList : Listable
{
    public Sprite[] images;
    
    private SpriteRenderer imageHolder;

    public override void Start() {
        base.Start();

        imageHolder = GetComponent<SpriteRenderer>();
    }

    public override void GoNext()
    {
        if (currentIndex + 1 >= images.Length) {
            currentIndex = 0;
        } else {
            currentIndex++;
        }

        RegenerateImage();
    }

    public override void GoPrevious()
    {
        if (currentIndex - 1 <= 0) {
            currentIndex = images.Length - 1;
        } else {
            currentIndex--;
        }

        RegenerateImage();
    }

    private void RegenerateImage() {
        imageHolder.sprite = images[currentIndex];
    }
}
