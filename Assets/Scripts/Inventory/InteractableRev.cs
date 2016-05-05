using UnityEngine;
using System.Collections;

/**
 * Script for anything we can click on in the game, stuff to look at as well as items to pick up and characters to talk to
 */
public class InteractableRev : MonoBehaviour {

//	public bool pickUp;

	private Animator animator;

	public bool justLook;
	public string lookDescription;

	public float interactDistance = 1; // probably the same for all objects, but can be changed here. // Rev: Collision sphere? See below

	private TextMesh actionLine; // Rev: Happy to rename this - it's the constructed sentence that describes what left-clicking will do. =)
	private TextMesh actionLineShadow;

	//private TextMesh sayChar;
	//private TextMesh sayCharShadow;

	private PlayerMouseControl playMousCont;
	private Camera uiCamera;
	private Inventory inventory;

	public float 	readingTime = 1000.0f;
	public float 	actionLineTime = 0.0f;
	private bool 	actionLineReset = false;

	public Vector3 invTargetRotation = new Vector3 (0.0f,0.0f,0.0f);
	public Vector3 invTargetScale = new Vector3(1.0f,1.0f,1.0f);
	public float invTargetYPos = 0.0f;
	
	private bool allowDrag;

	// Conversation stuff (no reason we can't theoretically start a conversation with an object)
	public TextMesh sayNPC; 
	public TextMesh sayNPCShadow;

	public bool		hasStandHere = false;
	public Vector3	standHere 	= new Vector3(0,0,0);
	public bool		haslookHere = false;
	public Vector3	lookHere 	= new Vector3(0,0,0);

	public MatchEvent[] events;

	private string[] defaultResponses = new string[]{"That doesn't do anything.", "Hmm... how?", "I don't see the point in that.", "No that wont help me.", "Huh?"};

	/**
	 * Class representing a single pairing of items. Each interactable holds a list of these
	 */
	[System.Serializable]
	public class MatchEvent {
		public string itemName;						// Name of Interactable to match with
		public string comment;						// What the player says
		public bool pickUp;							// pick up this item
		public bool destroyThis;					// destroy (remove) this item after this interaction
		public bool destroyOther;					// destroy (remove) the item used on this
		public GameObject[] newItemInInventory; 	// new item (prefab) to create in inventory, set to null if no item comes of it
		public DialogueConversation conversation;	// Conversation prefab to start
	}

	// Use this for initialization
	void Start () {

		animator = GetComponentInChildren<Animator> () as Animator;

		actionLine = GameObject.Find ("sayNeutral").GetComponent<TextMesh> ();
		actionLineShadow = GameObject.Find ("sayNeutralShadow").GetComponent<TextMesh> ();

		playMousCont = GameObject.Find ("Main Camera").GetComponent<PlayerMouseControl> ();

		uiCamera = GameObject.Find("InventoryCamera").GetComponent<Camera>();
		inventory = GameObject.Find ("Inventory").GetComponent<Inventory>();

		ResetActionLineTime ();

		// Instantiate conversation in case any of them are prefabs
		foreach(MatchEvent ev in events){
			if(ev.conversation != null){
				DialogueConversation convo = Instantiate(ev.conversation) as DialogueConversation;
				convo.name = convo.name.Replace ("(Clone)","");
				ev.conversation = convo;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(!GameFlow.instance.conversationClickThrough){
			// Advance text automatically
			if(readingTime < GameFlow.instance.readingSpeed){
				readingTime += Time.deltaTime;
			}
		}

		if(readingTime >= GameFlow.instance.readingSpeed){
			hideText();
		}

		if(actionLineTime < GameFlow.instance.readingSpeed){
			actionLineTime += Time.deltaTime;
		}

		if(actionLineTime >= GameFlow.instance.readingSpeed && actionLineReset){
			hideActionLine();
			actionLineReset = false;
		}


		// If the player clicks, max out the reading time so we skip to the next text
		if(Input.GetButtonDown("Fire1")){
			if(GameFlow.instance.conversationClickThrough)
				readingTime = float.MaxValue;
			else if (!GameFlow.instance.inputPaused)
				readingTime = float.MaxValue;
		}

	}

	void OnMouseDown() {
		if(tag == "Inventory"){
			allowDrag = true;
		}
		if (actionLine != null) {
			actionLine.text = null;
			actionLineShadow.text = null;
			playMousCont.CursorNormal();
		}
	}

	void OnMouseDrag() {
		if(tag == "Inventory" && allowDrag){
			GetComponent<Collider>().enabled = false;
			Vector3 worldPoint = uiCamera.ScreenToWorldPoint(Input.mousePosition);
			transform.position = new Vector3(worldPoint.x, worldPoint.y, 1);
		}
	}

	void OnMouseUp() {
		allowDrag = false;
		inventory.settleItems();
		//collider.enabled = true;
	}

	void OnMouseOver () {
		playMousCont.CursorHighlight();
	}

	void OnMouseEnter () {
		if(actionLine != null) {
			ResetActionLineTime();
			actionLine.text = gameObject.name;
			actionLineShadow.text = gameObject.name;
		}
	}
	
	void OnMouseExit () {
		if (actionLine != null) {
			actionLine.text = null;
			actionLineShadow.text = null;
			// actionLineReset = false;
		}
		playMousCont.CursorNormal();
	}

	/**
	 * Player tries to use something on this item. 
	 * 
	 * Look through all this item's events and see if we have a match.	
	 * 
	 */
	public void Interact(InteractableRev otherItem){
		// Send a broadcast for this interaction, even if nothing happens, we can use this for debugging and for making quick
		// reactions in any other object that may want to know.
		//
		// The event name is both item names combined
		Messenger<string>.Broadcast("event", (otherItem.name + name));


		// See if we have any events for this item
		foreach(MatchEvent matchEvent in events){
			if(matchEvent.itemName.Equals(otherItem.name)){
				// Player comment
				GameFlow.instance.playerSay(matchEvent.comment);

				// Picking items up
				if(matchEvent.pickUp) inventory.addItem(gameObject);

				// Destroying items
				if(matchEvent.destroyThis) {
					gameObject.SetActive(false); // we just deactivate items for now
				}
				if(matchEvent.destroyOther) {
					inventory.removeItem(otherItem.gameObject);
				}

				// New inventory item
				if(matchEvent.newItemInInventory != null){
					for(int i = 0; i < matchEvent.newItemInInventory.Length; i ++){
						GameObject newItem = Instantiate(matchEvent.newItemInInventory[i]) as GameObject;
						newItem.name = newItem.name.Replace ("(Clone)",""); // Rev: Removes the automatically added suffix '(Clone)'.
						inventory.addItem(newItem);
					}
				}

				if(matchEvent.conversation != null){
					//DialogueConversation conversation = Instantiate(matchEvent.conversation) as DialogueConversation;
					//conversation.startConversation(this);

					matchEvent.conversation.startConversation(this);
				}

				return; // stop looking once we find a match, shouldn't have several events for one item
			}
		}

		// We don't have any events for this item, do something generic
		sayRandomNoGoMessage();
	}

	public void sayRandomNoGoMessage(){
		string response = defaultResponses[Random.Range(0, defaultResponses.Length)];
		GameFlow.instance.playerSay(response);
	}

	public void ResetReadingTime () {
		readingTime = 0.0f;
		//Debug.Log ("Resetting timer for reading text speed");
	}
	
	public void ResetActionLineTime() {
		actionLineTime = 0.0f;
		//actionLineReset = true;
	}
	
	public bool isTalking(){
		return readingTime < GameFlow.instance.readingSpeed;
	}

	public void removeEventWithConvo(DialogueConversation conversation){
		removeEventWithConvoName(conversation.name);
	}

	public void removeEventWithConvoName(string convoName){
		for(int n = 0 ; n < events.Length ; n++){
			MatchEvent ev = events[n];
			if(ev.conversation != null)
			if(convoName.Equals(ev.conversation.name)){
				ev.itemName = "DELETED";
			}
		}
	}

	public void say(string text){
		if(sayNPC == null) return;
		text = text.Replace ("NUULINE", "\n");
		sayNPC.text = text;
		sayNPCShadow.text = text;
		ResetReadingTime();
	}

	private void hideText(){
		if(sayNPC == null) return;
		sayNPC.text = "";
		sayNPCShadow.text = "";
	}

	private void hideActionLine(){
		if(actionLine == null) return;
		actionLine.text = "";
		actionLineShadow.text = "";
	}

	public bool withinRange(Vector3 playerPosition){	// Kristian, does it make sense to replace this with a sphere collider test? More performant, visual.
		Vector3 thisPosition = new Vector3(transform.position.x, 0.0f, transform.position.z);
		playerPosition = new Vector3(playerPosition.x, 0.0f, playerPosition.z);
		return Vector3.Distance(thisPosition, playerPosition) <= interactDistance;
	}

	public void headBobble() {
		animator.SetTrigger ("headBobble");
	}

	public void elkeSitDown(){
		animator.SetBool ("sitting", true);
		Debug.Log ("Sitting anim activated.");
	}

	public void elkeStandUp(){
		animator.SetBool ("sitting", false);
		Debug.Log ("Sitting anim deactivated.");
	}

	public void elkeChop(){
		animator.SetTrigger ("chopping");
		Debug.Log ("Chopping anim triggered.");
	}

	public void elkePound(){
		animator.SetTrigger ("pounding");
		Debug.Log ("Pounding anim triggered.");
	}

	public void elkeThrow(){
		animator.SetTrigger ("throw");
		Debug.Log ("Throw anim triggered.");
	}

	public void royoIsBrushing (){
		animator.SetBool ("brushing", true);
	}

	public void royoNotBrushing (){
		animator.SetBool ("brushing", false);
	}

	public void abnerPat(){
		animator.SetTrigger ("abnerPat");
	}
}
