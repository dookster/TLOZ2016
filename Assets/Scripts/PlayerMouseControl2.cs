using UnityEngine;
using System.Collections;

public class PlayerMouseControl2 : MonoBehaviour {
	
	public	GameObject		walkTo;
	
	public	NavMeshAgent	character01;
	
	public 	Camera 	cameraA;
	public	Camera	cameraUI;
	
	public	float	rayDistance	=	25.00f;

	private InteractableKT currentTarget; // If this isn't null the player should move towards it and interact when close enough
	private Inventory inventory;

	// Use this for initialization
	void Start () {
		inventory = GameObject.Find("Inventory").GetComponent<Inventory>() as Inventory;
	}
	
	// Update is called once per frame
	void Update () {
		/*
		 * Clicking on stuff is ordered by tags. 
		 * 
		 * "Level": 		Static background stuff, walls, floors etc. Clicking on it will try to move the PC there
		 * "Interactable":	Stuff we can use, talk to or pick up 
		 * "Inventory": 	Stuff in the inventory, interactables that are picked up switch their tag to this (and back if put down of course)
		 * 
		 */ 

		// If there's a target, interact with it if we're close enough
		if(currentTarget != null){
			if(currentTarget.withinRange(character01.transform.position)){
				if(currentTarget.pickUp){
					// Move to inventory
					inventory.addItem(currentTarget.gameObject);
				} else {
					// Let the target do whatever it does when interacting (e.g. start conversation with npc?)
					currentTarget.Interact();
				}

				// Target reached, remove it
				currentTarget = null;
			}
		}

		// Handle click input
		Ray ray = cameraA.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		// Click input on UI
		Ray uiRay = cameraUI.ScreenPointToRay(Input.mousePosition);

		Debug.DrawRay(cameraA.transform.position, ray.direction * rayDistance, Color.red);

		// Clicking in the game view
		if (Physics.Raycast(ray, out hit)){
			// Interactables

			if (hit.transform.tag == "Interactable"){
				InteractableKT interactable = hit.transform.GetComponent("InteractableKT") as InteractableKT;

				if (interactable == null) Debug.LogError("Something has the Interactable tag, but not the script");

				if (Input.GetButtonDown ("Fire1")){
					if(interactable.justLook){
						// Don't move, just look at it
						Debug.Log ("I'm looking at " + interactable.name + " and it's " + interactable.lookDescription);
					} else {
						// Target and move towards it
						currentTarget = interactable;
						walkTo.transform.position	= hit.point;
						character01.destination 	= hit.point;
					}
				}
							
			} else if (Input.GetButtonDown ("Fire1") && hit.transform.tag == "Level"){
				// Clicked on something in the level, clear target and walks towards hit position
				currentTarget = null;

				walkTo.transform.position	= hit.point;			
				character01.destination 	= hit.point;
			}

		}

		// Clicking UI stuff
		if (Physics.Raycast(uiRay, out hit)){
			if (Input.GetButton("Fire1") && hit.transform.tag == "Inventory"){
				// Clicking something in the inventory
				// handle click and drag?
				
				// Just testing remove from inventory for now
				inventory.removeItem(hit.transform.gameObject);
				
			}
		}
	}

	                         
}
