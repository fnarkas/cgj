using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointEditor : MonoBehaviour {

    public int Number;
	// Use this for initialization
	void Start () {
		
	}
	
	public /// <summary>
	/// Called when the script is loaded or a value is changed in the
	/// inspector (Called in the editor only).
	/// </summary>
	void OnValidate()
	{
		GetComponentInChildren<Text>().text = "" +Number;
	}
	// Update is called once per frame
	void Update () {
		
	}
}
