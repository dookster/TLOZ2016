using UnityEngine;
using System.Collections;

public class RoyoBlock : MonoBehaviour {

	public float threshold;
	public Transform player;
	public InteractableRev royo;
	public string warning;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(transform.position, player.position) < threshold){
			royo.say(warning);
		}
	}
}
