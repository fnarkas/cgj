using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartController : MonoBehaviour {
	public GameObject heartPrefab;
	public GameObject[] heartsLeft;
	public GameObject[] heartsRight;

	// Use this for initialization
	void Start () {
		int nLives = 3;
		heartsLeft = new GameObject[nLives];
		heartsRight = new GameObject[nLives];
		var container = InitHearts(nLives, heartsLeft);	
		var container2 = InitHearts(nLives, heartsRight);
		RectTransform rect = container2.GetComponent<RectTransform>();
		rect.pivot = new Vector2(0.0f, 1.0f);
		rect.anchoredPosition = Vector2.zero;
		rect.anchorMin = new Vector2(0.5f,0.87f);
		rect.anchorMax = new Vector2(1.0f,1.00f);
	}

    private GameObject InitHearts(int nLives, GameObject[] hearts)
    {
		GameObject lifeContainer = new GameObject("LifeContainer");
		lifeContainer.AddComponent<RectTransform>();
		RectTransform rect = lifeContainer.GetComponent<RectTransform>();
		rect.anchoredPosition = Vector2.zero;
		rect.pivot = new Vector2(0.0f, 1.0f);
		rect.anchorMin = new Vector2(0.0f,0.87f);
		rect.anchorMax = new Vector2(0.5f,1.00f);
		lifeContainer.transform.SetParent(transform, false);
		float x =0;
		for(int i=0; i< nLives; i++){
			var heart = GameObject.Instantiate(heartPrefab);
			heart.transform.SetParent(lifeContainer.transform,false);
			rect = heart.GetComponent<RectTransform>();
			rect.pivot = new Vector2(0.0f, 1.0f);
			rect.anchoredPosition = new Vector2(x, 0.0f);
			x += rect.sizeDelta.x;
			hearts[i] = heart;
		}
		return lifeContainer;
    }

    // Update is called once per frame
    void Update () {
		
	}

    internal void SetLives(int lives)
    {
		for(int i=0; i < heartsLeft.Length; i++){
			if(i < lives){
				heartsLeft[i].SetActive(true);
				heartsRight[i].SetActive(true);
			}
			else{
				heartsLeft[i].SetActive(false);
				heartsRight[i].SetActive(false);
			}
		}
    }
}
