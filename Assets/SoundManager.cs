using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

  private AudioSource source;
  public AudioClip[] PickUp;
  public AudioClip[] LvlTheme;
  public AudioClip Death;
	// Use this for initialization
	void Start () {

    source = GameObject.Find("Audio Source").GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update () {

	}

  public void PlayPickup(int nbr) {

    source.PlayOneShot(PickUp[nbr]);
  }

  public void PlayDeath() {

    source.PlayOneShot(Death);
  }

  public void PlayLvlTheme(int nbr) {

    source.PlayOneShot(LvlTheme[nbr]);
  }
}
