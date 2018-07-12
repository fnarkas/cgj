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


	public void ShowGhosts(List<GameObject> list){
		int n = list.Count;
		int i =0;
		foreach(var ghost in ghosts){
			ghost.SetActive(false);
			foreach(var g in list){
				if(g.name == ghost.name){
				if(i < n)
					ghost.SetActive(true);
				}
			}
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
