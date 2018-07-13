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

	public GameObject nothing;
	public GameObject tacos;


	public void ShowGhosts(List<GameObject> list){
		int n = list.Count;
		int i =0;

		foreach(var ghost in ghosts){
			ghost.SetActive(false);
			// Debug.Log("Ghost: " + ghost.name);
			foreach(var g in list){
			// Debug.Log("g: " + g.name);
				if(g.name == ghost.name){
				if(i < n){
					ghost.SetActive(true);
					i++;
					}
				}
							}
		}
	}

	private GameObject[] GetKeys(){
		GameObject[] keys = {w, a, s, d, up, down, left, right};
		return keys;
	}

	private void DisableAllLeft(){
		GameObject[] keys = { w, a, s, d};
		DisableAll(keys);
	}
	private void DisableAllRight(){
		GameObject[] keys = { up, down, left, right};
		DisableAll(keys);
	}

	public void ShowNone(){
		DisableAll(GetKeys());
		nothing.SetActive(true);
	}

	private void DisableAll(GameObject[] keys){
		foreach(var key in keys){
			key.SetActive(false);
		}
	}
public void ShowVerticalLeft(){
		DisableAllRight();
		a.SetActive(false);
		d.SetActive(false);
		s.SetActive(true);
		w.SetActive(true);
		nothing.SetActive(false);
	}

	public void ShowVerticalRight(){
		DisableAllLeft();
		up.SetActive(true);
		down.SetActive(true);
		left.SetActive(false);
		right.SetActive(false);
		nothing.SetActive(false);
	}
	public void ShowHorizontalLeft(){
		a.SetActive(true);
		d.SetActive(true);
		w.SetActive(false);
		s.SetActive(false);
		DisableAllRight();
		nothing.SetActive(false);
	}

	public void ShowHorizontalRight(){
		DisableAllLeft();
		left.SetActive(true);
		right.SetActive(true);
		up.SetActive(false);
		down.SetActive(false);
		nothing.SetActive(false);
	}
	public void ShowAllRight(){
		DisableAllLeft();
		left.SetActive(true);
		right.SetActive(true);
		up.SetActive(true);
		down.SetActive(true);
		nothing.SetActive(false);
	}
	public void ShowAllLeft(){
		w.SetActive(true);
		a.SetActive(true);
		s.SetActive(true);
		d.SetActive(true);
		DisableAllRight();
		nothing.SetActive(false);
	}

	public void ShowTacos(){
		tacos.SetActive(true);
	}
	public void HideTacos(){
		tacos.SetActive(false);
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}
}
