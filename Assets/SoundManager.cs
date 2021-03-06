﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

  private AudioSource source;
  public AudioClip[] PickUp;
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
      source = GameObject.Find("Audio Source Effects").GetComponent<AudioSource>();
    }
    return source;
  }

  public void PlayPickup(int nbr) {

    GetSource().PlayOneShot(PickUp[nbr]);
  }

  public void PlayDeath() {
    GetSource().Stop();
    GetSource().PlayOneShot(Death);
  }

  public bool isPlaying() {
    return GetSource().isPlaying;
  }
}
