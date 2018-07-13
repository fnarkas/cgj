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

    private SoundManager sourceEffects;
    private SoundManagerMusic sourceMusic;

    String verticalPlayer = "";
    String horizontalPlayer = "";

    AI _ai;
    public enum Controls { LeftAll, RightAll, LeftHorizontal, LeftVertical }

    void Awake()
    {
        // Use Awake to skip race condition with GameManager
        // who also tries to set this value
        RandomizePlayerControls(false);
    }

    // Use this for initialization
    void Start()
    {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _ai = GameObject.FindObjectOfType<AI>();
        _dest = transform.position;
        sourceEffects = GameObject.Find("Audio Source Effects").GetComponent<SoundManager>();
        sourceMusic = GameObject.Find("Audio Source Music").GetComponent<SoundManagerMusic>();
    }

    public Controls RandomizePlayerControls(bool randomize = true)
    {
        Controls controls;
      if (randomize) {
        float rand = UnityEngine.Random.value;
        if(rand > 0.5){
          horizontalPlayer = "P1Horizontal";
          verticalPlayer = "P2Vertical";
          controls = Controls.LeftVertical;
        } else {
          horizontalPlayer = "P2Horizontal";
          verticalPlayer = "P1Vertical";
          controls = Controls.LeftHorizontal;
        }
      } else {
        horizontalPlayer = "P1Horizontal";
        verticalPlayer = "P1Vertical";
        controls = Controls.LeftAll;
      }
      return controls;
    }

    void Update(){
       if(GameManager.gameState == GameManager.GameState.Game){
        if (Input.GetAxis(horizontalPlayer) > 0) _nextDir = Vector2.right;
        if (Input.GetAxis(horizontalPlayer) < 0) _nextDir = -Vector2.right;
        if (Input.GetAxis(verticalPlayer) > 0) _nextDir = Vector2.up;
        if (Input.GetAxis(verticalPlayer) < 0) _nextDir = -Vector2.up;
       }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        switch (GameManager.gameState)
        {
            case GameManager.GameState.Game:
                Move();
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
        // Play death tune
        sourceMusic.StopPlaying();
        sourceEffects.PlayDeath();
        yield return new WaitForSeconds(1);
        GetComponent<Animator>().SetBool("Die", false);
        _deadPlaying = false;

        if (GameManager.lives <= 0)
        {
          GameManager.gameState = GameManager.GameState.GameOver;
        }

        else
            GM.ResetScene();
    }

    void Animate()
    {
        Vector2 dir = _dest - (Vector2)transform.position;
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
        Vector2 scale = transform.localScale;
        if(dir.x > 0.0f){
            scale.x = -Math.Abs(scale.x);
        }else if(dir.x < 0.0f){
            scale.x = Math.Abs(scale.x);
        }
        transform.localScale = scale;
    }

    bool Valid(Vector2 direction)
    {
        // cast line from 'next to pacman' to pacman
        // not from directly the center of next tile but just a little further from center of next tile
        Vector2 pos1 = transform.position;
        Vector2 pos2 = pos1;
        Vector2 pos3 = pos1;
        float distance = 0.45f;
        float spread = 0.55f;
        Vector2 dir1 = direction + new Vector2(direction.x * distance, direction.y * distance);
        Vector2 dir2 = dir1;
        Vector2 dir3 = dir1;
        //Horizontal movement
        if(Math.Abs(direction.x) > Math.Abs(direction.y)){
            pos1 += Vector2.up * spread;
            pos3 += Vector2.down *spread;
        }
        // Vertical movement
        else{
            pos1 += Vector2.left * spread;
            pos3 += Vector2.right *spread;
        }
        RaycastHit2D hit1 = Physics2D.Linecast(pos1 + dir1, pos1);
        RaycastHit2D hit2 = Physics2D.Linecast(pos2 + dir2, pos2);
        RaycastHit2D hit3 = Physics2D.Linecast(pos3 + dir3, pos3);
        // Debug.DrawLine(pos1, pos1+ dir1);
        Debug.DrawLine(pos2, pos2+ dir2);
        // Debug.DrawLine(pos3, pos3+ dir3);
        bool isEmpty = true;
        if(direction == Vector2.left)
            isEmpty = !_ai.IsWallLeft(pos2);
        if(direction == Vector2.right)
            isEmpty = !_ai.IsWallRight(pos2);
        if(direction == Vector2.up)
            isEmpty = !_ai.IsWallUp(pos2);
        if(direction == Vector2.down)
            isEmpty = !_ai.IsWallDown(pos2);
        bool colliderIsCheckpoint = hit2.collider.name == "checkpoint";
        return colliderIsCheckpoint || isEmpty;
    }

    public void ResetDestination()
    {
        _dest = new Vector2(15f, 11f);
        GetComponent<Animator>().SetFloat("DirX", 1);
        GetComponent<Animator>().SetFloat("DirY", 0);
    }

    void Move()
    {
        // move closer to destination
        Vector2 p = Vector2.MoveTowards(transform.position, _dest, speed);
        GetComponent<Rigidbody2D>().MovePosition(p);



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
