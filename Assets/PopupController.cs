using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour {
	public GameObject w;
	public GameObject a;
	public GameObject s;
	public GameObject d;
	public GameObject up;
	public GameObject down;
	public GameObject left;
	public GameObject right;

	public GameObject[] ghosts;

	public void ShowGhost(int n){
		int i =0;
		foreach(var ghost in ghosts){
			ghost.SetActive(false);
			if(i < n)
				ghost.SetActive(true);
			i++;
		}
	}	

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
