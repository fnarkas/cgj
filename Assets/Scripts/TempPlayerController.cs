using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerController : MonoBehaviour
{
    public static TempPlayerController instance;
    Rigidbody2D rb;
    public float moveSpeed = 4.5f, jumpForce = 15.5f;
    float minDistToGround, timer = 0, untilRestart = 1;
    bool isTouching;
    Vector2 dir;
    BoxCollider2D col;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        minDistToGround = col.bounds.extents.y + 0.05f;
    }

    private void Update()
    {
        rb.velocity = new Vector2(dir.x * moveSpeed * 100 * Time.deltaTime, rb.velocity.y);
        dir = Vector2.zero;

        if (Input.GetKey(KeyCode.D))
        {
            if (Grounded() || !isTouching)
                dir = Vector2.right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (Grounded() || !isTouching)
                dir = Vector2.left;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (Grounded() && rb.velocity.y == 0)
            {
                Jump();
            }
        }


    }

    public void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public bool Grounded()
    {

        if (RayHit(new Vector2(col.bounds.extents.x + rb.position.x - 0.03f, rb.position.y), Vector2.down, minDistToGround, "environment")
            || RayHit(new Vector2(-col.bounds.extents.x + rb.position.x, rb.position.y), Vector2.down, minDistToGround, "environment"))
        {
            return true;
        }
        else
            return false;
    }

    public bool RayHit(Vector2 startFrom, Vector2 dir, float dist, string tag)
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(startFrom, dir, dist);
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].transform.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
    }

    public void Death()
    {
        //placeholder death function
        Destroy(GetComponent<MeshRenderer>());
        StartCoroutine("RestartTimer");
    }

    IEnumerator RestartTimer()
    {
        yield return new WaitForSeconds(untilRestart);

        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        isTouching = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isTouching = false;

    }
}
