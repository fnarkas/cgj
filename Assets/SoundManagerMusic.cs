using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerMusic : MonoBehaviour {

  private AudioSource source;
  public AudioClip[] LvlTheme;
  public AudioClip[] LvlStartTheme;
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
      source = GameObject.Find("Audio Source Music").GetComponent<AudioSource>();
    }
    return source;
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

  public void StopPlaying()
  {
    GetSource().Stop();
  }
}
