using UnityEngine;
using System.Collections;

public class CharacterMoveDefault : MonoBehaviour {

	private NavMeshAgent nav;
	private Animator anim;

	public Vector3 navVelocity;
	public float navMag;

	public float damping = 0.1f;

	// Use this for initialization
	void Start () {
	
		nav = GetComponentInChildren<NavMeshAgent> ();
		anim = GetComponentInChildren<Animator> ();

	}
	
	// Update is called once per frame
	void Update () {
	
//		Debug.Log (nav.velocity);
//		Debug.Log (nav.velocity.magnitude);

		navVelocity = nav.velocity;
		navMag = nav.velocity.magnitude;

//		anim.SetFloat ("speed", nav.velocity.sqrMagnitude);
//		anim.SetFloat ("speed", (nav.velocity.x) + (nav.velocity.z), damping, Time.deltaTime);
		anim.SetFloat ("speed", nav.desiredVelocity.sqrMagnitude, damping, Time.deltaTime);
//		anim.SetFloat ("speed", nav.desiredVelocity.sqrMagnitude);
	}
}
