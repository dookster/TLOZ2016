using UnityEngine;
using System.Collections;

public class MajaFootsteps : MonoBehaviour {

	public AudioClip[] audCli;

	private int lastStep = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void PlayStep() {

		int step = 0;
		
		while (step == lastStep){	
			step = Random.Range (0, 6);
		}
		lastStep = step;
		GetComponent<AudioSource>().clip = audCli [step];
		GetComponent<AudioSource>().Play ();
	}
}
