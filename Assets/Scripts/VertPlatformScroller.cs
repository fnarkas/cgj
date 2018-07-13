using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertPlatformScroller : MonoBehaviour
{
    public float scrollSpeed = 0.25f, maxDistFromCam = 1;
    float camHeight, currentSpeed, catchupSpeed, camBottom, bgHeight;
    public Transform BG1, BG2;
    BoxCollider bgBox;
    Transform currentBG, otherBG;

    private void Start()
    {
        bgBox = BG1.GetComponent<BoxCollider>();
        bgHeight = bgBox.bounds.extents.y * 2;
        camHeight = PlatformerCamera.instance.Height;
        catchupSpeed = scrollSpeed * 4;
        currentBG = BG1;
        otherBG = BG2;
    }

    private void Update()
    {
        camBottom = Camera.main.transform.position.y - camHeight * 0.5f;
        float lavaTop = transform.position.y + GetComponent<Collider2D>().bounds.extents.y;
        ScrollingBackground();
        transform.position = new Vector2(transform.position.x, transform.position.y + Time.deltaTime * currentSpeed);

        if (lavaTop > camBottom - maxDistFromCam)
        {
            currentSpeed = scrollSpeed;
        }
        else if (lavaTop > camBottom - maxDistFromCam * 2)
        {
            currentSpeed = catchupSpeed;
        }
        else if (lavaTop < camBottom - maxDistFromCam * 2)
        {
            transform.position = new Vector2(0, camBottom - maxDistFromCam - GetComponent<Collider2D>().bounds.extents.y);
        }
       
    }

    void ScrollingBackground()
    {
        float bgBot = currentBG.position.y - bgBox.bounds.extents.y;

        Debug.DrawLine(new Vector2(-5, bgBot), new Vector2(5, bgBot), Color.blue);

        if(camBottom - bgBox.bounds.extents.y * 0.5f > bgBot) //if camBottom is higher than the bg half, move the bg beneith the current one, ontop of the current one.
        {
            otherBG.position += new Vector3(0, bgHeight * 2);
            Transform temp = otherBG;       //Switch current and other bg.
            otherBG = currentBG;
            currentBG = temp;
        }

        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            TempPlayerController.instance.Death();
        }
    }

}
