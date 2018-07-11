using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameManager _gm;
    private SoundManager source;

    private static int checkpointCounter = 0;

    // Use this for initialization
    void Start()
    {
        _gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        source = GameObject.Find("Audio Source").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
      Debug.Log("Collision with " + GetComponent<CheckpointEditor>().Number);
      if (other.name == "pacman")
      {
        gameObject.SetActive(false);
        _gm.NextCheckpoint();

        // Play audio clip
        source.PlayPickup(checkpointCounter);
        checkpointCounter++;
      }
    }
}
