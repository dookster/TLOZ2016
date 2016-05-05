using UnityEngine;
using System.Collections;

public class PlayerMouseControl : MonoBehaviour {
	
	public	GameObject		walkTo;
	
	public	NavMeshAgent	character01;
	public	Transform		characterTransform;
	public  InteractableRev playerInteractable; // interactable 'dummy' item to pass to items when we're not dragging anything, lets them choose to be picked up etc.

	public 	Camera 	cameraA;
	public	Camera	inventoryCamera; // Rev: Should set this up in Start, GameObject.Find etc.
	
	public	float	rayDistance	=	25.00f;

	private bool	haveClickedInv = false;

	public	bool		lookAt = false; // Rev: Variables for turning Maja towards the object or person she's looking at
	public 	Vector3		lookAtTarget;	// Rev: Possibly turn all this into a coroutine?
	public	float		lookAtAngle;
	public	float		lookAtSpeed;
	public	float		lookAtTimer;
	public	float		lookAtTimeTarget;
	
	private InteractableRev	currentWorldTarget; // If this isn't null the player should move towards it and interact when close enough
	[SerializeField] 
	private InteractableRev currentItemToUse; // Item we're currently dragging around from the inventory
	private Inventory		inventory;

	public Texture2D cursorTextureNormal; // Rev: Initial attempt to set up custom cursor, could be useful for dynamic cursor
	public Texture2D cursorTextureHighlight;
	public CursorMode cursorMode = CursorMode.Auto;
	public Vector2 hotSpot;

	public Vector2 mouseScreenPos; // Rev: Left this public to easily make screen measurements from inspector

	private InventoryCamera inventoryCam;

	// Use this for initialization
	void Start () {
		inventory = GameObject.Find ("Inventory").GetComponent<Inventory> () as Inventory; // Rev: Why 'as Inventory'? Unfamiliar!

		hotSpot = new Vector2(cursorTextureNormal.width / 2,cursorTextureNormal.height / 2);
		Cursor.SetCursor (cursorTextureNormal, hotSpot, cursorMode); // Rev: More initial custom cursor code.

		inventoryCam = GameObject.Find ("InventoryCamera").GetComponent<InventoryCamera> ();

		currentItemToUse = playerInteractable;
	}
	
	// Update is called once per frame
	void Update () {

		// KT: Show the inventory when pointer is in top 1/8th of the screen
		if (Input.mousePosition.y < Screen.height - Screen.height / 8){
			inventoryCam.OpenInventory();
		}else if (Input.mousePosition.y > Screen.height / 8 && !GameFlow.instance.inputPaused){
			inventoryCam.CloseInventory();
		}

		/*
		 * Clicking on stuff is ordered by tags. 
		 * 
		 * "Level": 		Static background stuff, walls, floors etc. Clicking on it will try to move the PC there
		 * "Interactable":	Stuff we can use, talk to or pick up 
		 * "Inventory": 	Stuff in the inventory, interactables that are picked up switch their tag to this (and back if put down of course)
		 * 
		 */ 

		// If there's a target, interact with it if we're close enough
		if(currentWorldTarget != null){
			if(currentWorldTarget.withinRange(character01.transform.position)){ // Rev: Sphere collider?
				// Let the target do whatever it does when interacting, picking up is handled by the items themselves
				lookAtTarget = currentWorldTarget.transform.position; // Rev: Get pre-picked-up position for Maja to turn towards
				currentWorldTarget.Interact(currentItemToUse);
				// Target reached, remove it from the target variable
				currentWorldTarget = null;
				lookAt = true;	// Rev: Triggers the timer looks to make Maja turn towards the object position.
			}
		}

		if (lookAtTimer < lookAtTimeTarget && lookAt){
			lookAtTimer += Time.deltaTime;
			TurnToFace(lookAtTarget);
			character01.updateRotation = false;
		} 
		
		if (lookAtTimer >= lookAtTimeTarget && lookAt){
			lookAt = false;
			lookAtTimer = 0.0f;
			character01.updateRotation = true;
		}

		// !!! Skipping almost all updates with this, could be handled better
		// Ignore input here while we're talking
		if(GameFlow.instance.inputPaused) return;

		// Handle click input
		Ray ray = cameraA.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		// Click input on UI
		Ray uiRay = inventoryCamera.ScreenPointToRay (Input.mousePosition);

		Debug.DrawRay(cameraA.transform.position, ray.direction * rayDistance, Color.red);

		// Clicking in the game view
		if(Physics.Raycast(ray, out hit)){
			// Interactables

			if (hit.transform.tag == "Interactable"){
				InteractableRev interactable = hit.transform.GetComponent("InteractableRev") as InteractableRev; // Rev: Is this casting just to be certain?

				if(interactable == null) Debug.LogError("Something has the Interactable tag, but not the script");

				if (Input.GetButtonDown ("Fire1")){
					currentItemToUse = playerInteractable;
					if(interactable.justLook){
						// Don't move, just look at it 
						// Rev: Other times, we want them to walk and talk.
						Debug.Log ("I'm looking at " + interactable.name + " and it's " + interactable.lookDescription); // Rev: Should probably reduce this to description, for flexibility
					} else {
						// Target and move towards it
						currentWorldTarget = interactable;
						walkTo.transform.position	= hit.point;
						if(currentWorldTarget.hasStandHere){
							character01.destination		= currentWorldTarget.standHere;
						}else{
							character01.destination 	= hit.point;
						}

					}
				}

				// Dropping an item on something in the world
				if(Input.GetButtonUp("Fire1") && currentItemToUse != null){
					currentWorldTarget = interactable;
					walkTo.transform.position	= hit.point;

					if(currentWorldTarget.hasStandHere){
						character01.destination		= currentWorldTarget.standHere;
					}else{
						character01.destination 	= hit.point;
					}

					inventory.settleItemsWithoutAnimation();
				}
			}

			// Clicking UI stuff
			else if (Physics.Raycast (uiRay, out hit)){
				if(hit.transform.tag == "Inventory"){
					InteractableRev interactable = hit.transform.GetComponent("InteractableRev") as InteractableRev;
					if(Input.GetButtonUp("Fire1") && currentItemToUse != interactable){
						// Dragging inventory stuff on inventory stuff
						Debug.Log("Mixing stuff in inventory");
						interactable.Interact(currentItemToUse);
					}
					else if(Input.GetButtonDown ("Fire1") && !haveClickedInv){
						// Clicking something in the inventory?
						// KT: moving the item during click&drag is handled in the Interactable class, 
						//     here we handle interaction when dropping an item onto another
						currentItemToUse = interactable;
						haveClickedInv = true;
					}
				}
			} else if (Input.GetButtonDown ("Fire1")){
				// Clicked on 'nothing', clear target and walks towards it
				currentWorldTarget = null;
				currentItemToUse = playerInteractable;
				
				walkTo.transform.position	= hit.point;			
				character01.destination 	= hit.point;
			}
		}

		if (Input.GetButtonUp ("Fire1")){
			haveClickedInv = false;
			//currentItemToUse = null;
		}

		if (Input.GetButtonDown("Fire2")){
			// character01.updatePosition = !character01.updatePosition;
			character01.updateRotation = !character01.updateRotation;
		}


	}

	private void TurnToFace (Vector3 target){ // Rev: Checks angle between Maja's forward vector and target. If larger than goal, slerps towards goal.

		Vector3 relativePos = target - characterTransform.position;
		Vector3 forward = characterTransform.forward;
		
		if(Vector3.Angle(target, forward) > lookAtAngle){
			Quaternion rotation = Quaternion.LookRotation(relativePos); // Rev: NavMeshAgent.UpdateRotation has to be turned off before this'll work
			rotation.x = 0.0f;
			rotation.z = 0.0f;
			characterTransform.rotation = Quaternion.Slerp(characterTransform.rotation, rotation, GameFlow.instance.dTimeModified * lookAtSpeed);
		}
	}

	public void CursorHighlight(){
		Cursor.SetCursor (cursorTextureHighlight, hotSpot, cursorMode); // Rev: More initial custom cursor code
	}

	public void CursorNormal() {
		Cursor.SetCursor (cursorTextureNormal, hotSpot, cursorMode); // Rev: More initial custom cursor code
	}


}
