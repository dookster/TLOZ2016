using UnityEngine;
using System.Collections;

public class SimpleAudioPlay : MonoBehaviour {

	private AudioSource attachedAudio;

	// Use this for initialization
	void Start () {
	
		attachedAudio = GetComponent<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetKeyDown(KeyCode.P) && !attachedAudio.isPlaying){
			attachedAudio.Play();
		}else if(Input.GetKeyDown(KeyCode.P) && attachedAudio.isPlaying){
			attachedAudio.Pause();
		}
	}
}
