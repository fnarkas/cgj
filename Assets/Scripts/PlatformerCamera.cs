using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerCamera : MonoBehaviour
{
    public static PlatformerCamera instance;
    Transform Player;
    Vector3 targetPos;
    public float cameraSmoothing = 500, speedMultiplier = 0.5f;
    public GameObject leftBorder, rightBorder;
    [HideInInspector]
    public float Width, Height;


    private void Awake()
    {
        instance = this;
        Height = Camera.main.orthographicSize * 2;
        Width = Height * Screen.width / Screen.height;

    }

    private void Start()
    {
        Player = TempPlayerController.instance.transform;
    }

    private void Update()
    {
        float borderY = Mathf.Lerp(Player.position.y, transform.position.y, 0.75f);
        leftBorder.transform.position = new Vector2(-Width * 0.5f, borderY);
        rightBorder.transform.position = new Vector2(Width * 0.5f, borderY);

        targetPos = new Vector3(transform.position.x, targetPos.y, -10)
        {
            y = Mathf.MoveTowards(targetPos.y, Player.position.y + Player.GetComponent<Rigidbody2D>().velocity.y * speedMultiplier, Time.deltaTime)
        };

        transform.position = Vector3.MoveTowards(transform.position, targetPos, cameraSmoothing * Time.deltaTime);


    }

}
