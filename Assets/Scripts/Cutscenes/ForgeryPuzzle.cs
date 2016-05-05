using UnityEngine;
using System.Collections;

public class ForgeryPuzzle : StoryElement {

	public Camera puzzleCam;


	// Use this for initialization
	void Start () {
		//puzzleCam = GetComponentInChildren<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void HandleEvent(string eventName){

		if(eventName == "One of the president's pensOrders with code list"){
			puzzleCam.gameObject.SetActive(true);
		}
		if(eventName == "ForgeryPuzzle"){
			puzzleCam.gameObject.SetActive(false);
		}
	}
}
