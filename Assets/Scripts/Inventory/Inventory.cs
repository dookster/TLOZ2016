using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Inventory : MonoBehaviour {

	public Transform anchor;
	public float itemDistance = 0.5f;
	public List<GameObject> startingItems = new List<GameObject>();
	public List<GameObject> items = new List<GameObject>();

	// Use this for initialization
	void Start () {
		foreach(GameObject item in startingItems){
			GameObject newItem = Instantiate(item) as GameObject;
			newItem.name = newItem.name.Replace ("(Clone)", "");
			addItem(newItem);
		}
		settleItemsWithoutAnimation();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void addItem(GameObject item){
		item.transform.parent = anchor;
		InteractableRev interact = item.GetComponent<InteractableRev> (); // Rev: Cache the item's InteractableRev component for speed
		BoxCollider interactCollider = item.GetComponent<BoxCollider> (); // Rev: Cache the item's BoxColider component for speed
		item.transform.localPosition = new Vector3(0 + itemDistance * items.Count, interact.invTargetYPos, -1.0f);
		item.transform.localScale = interact.invTargetScale;
		item.transform.localEulerAngles = interact.invTargetRotation;
		if(interactCollider){ 									// Rev: If we've found a box collider for the item...
//			interactCollider.center = interact.invCollidCent; 	// Rev: ...apply new center and size settings in the InteractableRev to the collider.
//			interactCollider.size = interact.invCollidSize;
			Destroy(interactCollider);
			item.gameObject.AddComponent<BoxCollider>();
		}
		item.layer = LayerMask.NameToLayer("UI");
		item.tag = "Inventory";
		items.Add(item);
		settleItems();
	}

	public void removeItem(GameObject item){
		// Move/destroy item?
		item.SetActive(false);

		items.Remove(item);
		settleItems();
	}

	/**
	 * After adding, moving or removing an item, set all items into their place
	 */
	public void settleItems(){
		// KT: First remove any inactive items
		for(int n = items.Count-1 ; n > 0 ; n--){
			if(!items[n].activeSelf){
				items.RemoveAt(n);
			}
		}
		for(int n = 0 ; n < items.Count ; n++){
			float yPos = items[n].GetComponent<InteractableRev>().invTargetYPos; // Rev: Get customised y position
			iTween.MoveTo(items[n], iTween.Hash("x", 0 + itemDistance * n, "y", yPos, "z", -1.0f, "time", 0.5f, "islocal", true, "oncomplete", "settled", "oncompletetarget", gameObject, "oncompleteparams", items[n]));
		}
	}

	public void settleItemsWithoutAnimation(){
		// KT: First remove any inactive items
		for(int n = items.Count-1 ; n > 0 ; n--){
			if(!items[n].activeSelf){
				items.RemoveAt(n);
			}
		}
		for(int n = 0 ; n < items.Count ; n++){
			float yPos = items[n].GetComponent<InteractableRev>().invTargetYPos;
			items[n].transform.localPosition = new Vector3(0 + itemDistance * n, yPos, -1.0f);
			settled(items[n]);
		}
	}
	/**
	 * We need to wait until items are settled before re-enabling their colliders or we can't
	 * detect two inventory items dragged onto each other (the top item's collider is enabled 
	 * the instant we lift the mouse button so we risk trying to combine it with itself)
	 */
	public void settled(GameObject thing){
		thing.GetComponent<Collider>().enabled = true;
	}

}
