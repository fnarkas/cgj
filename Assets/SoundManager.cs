using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

  private AudioSource source;
  public AudioClip[] PickUp;
  public AudioClip[] LvlTheme;
  public AudioClip[] LvlStartTheme;
  public AudioClip Death;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

  void Awake () {
  }

  private AudioSource GetSource() {
    if(source == null) {
      source = GetComponent<AudioSource>();
    }
    return source;
  }

  public void PlayPickup(int nbr) {

    GetSource().PlayOneShot(PickUp[nbr]);
  }

  public void PlayDeath() {

    GetSource().PlayOneShot(Death);
  }

  public void PlayLvlTheme(int nbr) {
    GetSource().PlayOneShot(LvlTheme[nbr]);
  }

  public void PlayLvlStartTheme(int nbr) {
    GetSource().Stop(); // Fix issue when changing level and double music
    GetSource().PlayOneShot(LvlStartTheme[nbr]);
  }

  public bool isPlaying() {
    return GetSource().isPlaying;
  }
}
