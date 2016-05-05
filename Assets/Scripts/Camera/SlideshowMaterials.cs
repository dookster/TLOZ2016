using UnityEngine;
using System.Collections;

public class SlideshowMaterials : MonoBehaviour {

	public Material[] slides;
	private MeshRenderer meshRend;

	// Use this for initialization
	void Start () {
	
		meshRend = GetComponent<MeshRenderer> ();

	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetButtonDown ("Fire2")){
			SwitchSlide();
			Debug.Log ("Switching slide...");
		}

	}

	public void SwitchSlide (int slideNum = 0){
		 meshRend.material = slides [slideNum];
	}
}
