using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour {

	public Transform target;
	public float zValue;

	public bool	attachedToCam = false;

	void Update() {
		Vector3 relativePos = target.position - transform.position;
		relativePos = new Vector3 (relativePos.x, relativePos.x, 90.0f);
		Quaternion rotation = Quaternion.LookRotation(relativePos);
		transform.rotation = rotation;

		if(attachedToCam){
			transform.position = new Vector3(transform.position.x, transform.position.y, target.position.z + zValue);
		}
	}
}