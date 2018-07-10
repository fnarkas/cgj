using System;
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float speed = 0.4f;
    Vector2 _dest = Vector2.zero;
    Vector2 _dir = Vector2.zero;
    Vector2 _nextDir = Vector2.zero;

    [Serializable]
    public class PointSprites
    {
        public GameObject[] pointSprites;
    }

    public PointSprites points;

    public static int killstreak = 0;

    // script handles
    private GameManager GM;

    private bool _deadPlaying = false;

    String verticalPlayer = ""; 
    String horizontalPlayer = ""; 

    // Use this for initialization
    void Start()
    {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _dest = transform.position;
        RandomizePlayerControls();
    }

    private void RandomizePlayerControls()
    {
        float rand = UnityEngine.Random.value;        
        if(rand > 0.5){
           horizontalPlayer = "P1Horizontal"; 
           verticalPlayer = "P2Vertical"; 
           Debug.Log("1 is Horizontal 2 is Vertival");
        } else {
           horizontalPlayer = "P2Horizontal"; 
           verticalPlayer = "P1Vertical"; 
           Debug.Log("1 is Vertival 2 is Horizontal");
        }
           horizontalPlayer = "P1Horizontal"; 
           verticalPlayer = "P1Vertical"; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (GameManager.gameState)
        {
            case GameManager.GameState.Game:
                ReadInputAndMove();
                Animate();
                break;

            case GameManager.GameState.Dead:
                if (!_deadPlaying)
                    StartCoroutine("PlayDeadAnimation");
                break;
        }


    }

    IEnumerator PlayDeadAnimation()
    {
        _deadPlaying = true;
        GetComponent<Animator>().SetBool("Die", true);
        yield return new WaitForSeconds(1);
        GetComponent<Animator>().SetBool("Die", false);
        _deadPlaying = false;

        if (GameManager.lives <= 0)
        {

        }

        else
            GM.ResetScene();
    }

    void Animate()
    {
        Vector2 dir = _dest - (Vector2)transform.position;
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
    }

    bool Valid(Vector2 direction)
    {
        // cast line from 'next to pacman' to pacman
        // not from directly the center of next tile but just a little further from center of next tile
        Vector2 pos = transform.position;
        float distance = 0.45f;
        float spread = 0.55f;
        Vector2 dir1 = direction + new Vector2(direction.x * distance, direction.y * distance);
        Vector2 dir2 = dir1;
        //Horizontal movement
        if(Math.Abs(direction.x) > Math.Abs(direction.y)){
            dir1 += Vector2.up * spread;
            dir2 += Vector2.down * spread;
        }
        // Vertical movement
        else{
            dir1 += Vector2.left * spread;
            dir2 += Vector2.right *spread;
        }
        RaycastHit2D hit1 = Physics2D.Linecast(pos + dir1, pos);
        RaycastHit2D hit2 = Physics2D.Linecast(pos + dir2, pos);
        Debug.DrawLine(pos, pos+ dir1);
        Debug.DrawLine(pos, pos+ dir2);
        bool colliderIsMe = hit2.collider == GetComponent<Collider2D>() && (hit1.collider == GetComponent<Collider2D>());
        bool colliderIsCheckpoint = hit2.collider.name == "checkpoint";
        return colliderIsMe || colliderIsCheckpoint;
    }

    public void ResetDestination()
    {
        _dest = new Vector2(15f, 11f);
        GetComponent<Animator>().SetFloat("DirX", 1);
        GetComponent<Animator>().SetFloat("DirY", 0);
    }

    void ReadInputAndMove()
    {
        // move closer to destination
        Vector2 p = Vector2.MoveTowards(transform.position, _dest, speed);
        GetComponent<Rigidbody2D>().MovePosition(p);

        String playerHorizontal = horizontalPlayer;
        String playerVertical = verticalPlayer;

        // get the next direction from keyboard
        if (Input.GetAxis(playerHorizontal) > 0) _nextDir = Vector2.right;
        if (Input.GetAxis(playerHorizontal) < 0) _nextDir = -Vector2.right;
        if (Input.GetAxis(playerVertical) > 0) _nextDir = Vector2.up;
        if (Input.GetAxis(playerVertical) < 0) _nextDir = -Vector2.up;

        // if pacman is in the center of a tile
        if (Vector2.Distance(_dest, transform.position) < 0.00001f)
        {
            if (Valid(_nextDir))
            {
                _dest = (Vector2)transform.position + _nextDir;
                _dir = _nextDir;
            }
            else   // if next direction is not valid
            {
                if (Valid(_dir))  // and the prev. direction is valid
                    _dest = (Vector2)transform.position + _dir;   // continue on that direction

                // otherwise, do nothing
            }
        }
    }

    public Vector2 getDir()
    {
        return _dir;
    }

    public void UpdateScore()
    {
        killstreak++;

        // limit killstreak at 4
        if (killstreak > 4) killstreak = 4;

        Instantiate(points.pointSprites[killstreak - 1], transform.position, Quaternion.identity);
        GameManager.score += (int)Mathf.Pow(2, killstreak) * 100;

    }
}
