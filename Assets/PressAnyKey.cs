using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressAnyKey : MonoBehaviour
{
    float scrollSpeed;
    private void Start()
    {
        scrollSpeed = VertPlatformScroller.instance.scrollSpeed;
        VertPlatformScroller.instance.scrollSpeed = 0;

    }

    private void Update()
    {
        ListenForAnyKey();
    }
    private void ListenForAnyKey()
    {
        if (Input.anyKeyDown)
        {
            gameObject.SetActive(false);
            VertPlatformScroller.instance.scrollSpeed = scrollSpeed;
        }
    }
}
