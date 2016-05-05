using UnityEngine;
using System.Collections;

public class InventoryCamera : MonoBehaviour {

	public float posClosed = -0.99f;
	public float posOpen = 0.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void CloseInventory(){
		iTween.MoveTo(gameObject, iTween.Hash("x", 163.84, "y", posClosed, "z", 0, "time", 0.5f, "islocal", true));
	}

	public void OpenInventory(){
		iTween.MoveTo(gameObject, iTween.Hash("x", 163.84, "y", posOpen, "z", 0, "time", 0.5f, "islocal", true));
	}
}
