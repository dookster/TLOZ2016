using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	[SerializeField]
	private Transform target;
	
	public float speed = 1.0f;

	public float zDistance;

	public float yDistance;

	public bool trackPlayer = true;
	
//	private bool wasLocked = false;

	// Use this for initialization
	void Start () {
		
		// target = GameObject.Find ("MajaLund").GetComponent<Transform> ();
		
	}
	
	// Update is called once per frame
	void Update () {
	
		if(trackPlayer){
			Vector3 modifiedTarget = new Vector3 (target.position.x, target.position.y + yDistance, target.position.z + zDistance);
			transform.position = Vector3.Lerp (transform.position, modifiedTarget, GameFlow.instance.dTimeModified * speed);
		}


//		if (Input.GetKeyDown("escape"))
//			Screen.lockCursor = false;
//		
//		if (!Screen.lockCursor && wasLocked) {
//			wasLocked = false;
//			DidUnlockCursor();
//		} else
//		if (Screen.lockCursor && !wasLocked) {
//			wasLocked = true;
//			DidLockCursor();
//		}

	}

//	void DidLockCursor() {
//		Debug.Log("Locking cursor");
//		guiTexture.enabled = false;
//	}
//	void DidUnlockCursor() {
//		Debug.Log("Unlocking cursor");
//		guiTexture.enabled = true;
//	}
//	void OnMouseDown() {
//		Screen.lockCursor = true;
//	}
}