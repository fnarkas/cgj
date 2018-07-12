using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameManager _gm;

    // Use this for initialization
    void Start()
    {
        _gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
      if (other.name == "pacman")
      {
        gameObject.SetActive(false);
        _gm.NextCheckpoint();
      }
    }
}
