using UnityEngine;
using System.Collections;

public class minorMakingCoffee : MonoBehaviour {

	// private AudioSource audSource;
	public	AudioClip[] audClips;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Brewing () {
		Debug.Log ("Brewing coffee...!");
		GetComponent<AudioSource>().clip = audClips [0];
		GetComponent<AudioSource>().Play ();
	}

	void Pouring () {
		Debug.Log ("Pouring coffee...");
		GetComponent<AudioSource>().clip = audClips [1];
		GetComponent<AudioSource>().Play ();
	}

	void TakeCupOfCoffee () {
		Debug.Log ("Taking cup...");
		GetComponent<AudioSource>().clip = audClips [2];
		GetComponent<AudioSource>().Play ();
	}
}
