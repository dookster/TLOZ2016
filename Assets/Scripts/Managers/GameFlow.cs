using UnityEngine;
using System.Collections;

public class GameFlow : MonoBehaviour {

	// Creating Singleton variables
	private static GameFlow _instance;

	public static GameFlow instance{
		get{
			if(_instance == null){
				_instance = GameObject.FindObjectOfType<GameFlow>();
				// Tell Unity not to destroy this object when loading a new scene!
				DontDestroyOnLoad(_instance.gameObject);
			}
			return _instance;
		}
	}

	[SerializeField]	float	deltaTimeModifier = 1.0f;
	[Range(0.0f, 1.0f)]
	[SerializeField]	float	dTimeTarget = 1.0f;
	public	float	dTimeTargetSpeed = 1.0f;
	public	float	dTimeModified; // This should be a property!
	[SerializeField]	bool	pause = false;


	public float readingSpeed = 2.0f;
	public bool conversationClickThrough; // If true, wait for the player to click before advancing conversations

	public ConversationUI conversationUI;

	public bool inputPaused; // Set to true to keep the player from moving the character, e.g. in a conversation

	private TextMesh actionLine; // Rev: Happy to rename this - it's the constructed sentence that describes what left-clicking will do. =)
	private TextMesh actionLineShadow;

	private TextMesh 	debugStatus;
	private bool		debugStatusVisible = false;

	public float 	actionLineTime = 0.0f;
	public bool 	actionLineReset = false;

	public AudioClip[] audClips;

	public CameraFollow camFollow;

	private Inventory inv;
	public GameObject testInvItem;

	public GameObject spawnEmptyCup; // Rev: Spawn so you can keep giving coffee to Royo
	public GameObject forgedOrders; 

	private GameObject faderCard;
	private GameObject intro01;
	private SlideshowMaterials intro01SlideshowMats;

	// Rev: Local references to actors for nav, say and turn actions in cutscenes
	private GameObject		objMaja;
	private	NavMeshAgent 	navMaja;
	public 	InteractableRev playerInteractable;

	private	GameObject		objAbner;
	private	NavMeshAgent 	navAbner;
	public 	InteractableRev abnerInteractable;

	private GameObject		objRoyo;
	private NavMeshAgent 	navRoyo;
	public 	InteractableRev royoInteractable;

	private GameObject		objElke;
	private NavMeshAgent 	navElke;
	public 	InteractableRev elkeInteractable;

	// Rev: Bools set by functions used to set up game events - bools are displayed in 3D Text.
	public bool		cutsceneIntro 		= false;
	public bool		eavesdropSetup 		= false;
	public bool		cutsceneEnvelope 	= false;
	public bool		sealedSetup			= false;
	public bool		cutsceneMortar		= false;
	public bool		whiskeySetup		= false;
	public bool		computerSetup		= false;
	public bool		cutsceneConfront	= false;
	public bool		finaleSetup			= false;
	public bool		cutsceneOutro		= false;
	public bool		cutsceneCredits		= false;


	// Transforms that parent different versions of objects, enable/disable these as the story progresses
	public Transform preEavesdropNode;
	public Transform eavesdropNode;
	public Transform forgeryNode;
	public Transform forgeryNode2;
	public Transform preBoomNode;
	
	void OnEnable(){
		// Listen for any broadcasts of the type 'event'
		Messenger<string>.AddListener("event", HandleEvent);
	}

	void OnDestroy(){
		Messenger<string>.RemoveListener("event", HandleEvent);
	}

	// Use this for initialization
	void Start () {
		//sayChar = GameObject.Find ("sayMaja").GetComponent<TextMesh> ();
		//sayCharShadow = GameObject.Find ("sayMajaShadow").GetComponent<TextMesh> ();

		// Rev: References to character navMeshAgents so cutscenes can direct the characters...
		// Rev: ...references to interactables so cutscenes can prompt characters to say things...
		// Rev: ...references to character objects so they can be turned in a specific direction.
		objMaja					= GameObject.Find ("MajaLund");
		playerInteractable 		= GameObject.Find ("Player").GetComponent<InteractableRev>();
		navMaja 				= objMaja.GetComponent<NavMeshAgent> ();

		objAbner				= GameObject.Find ("Abner Hall");
		abnerInteractable		= objAbner.GetComponent<InteractableRev>();
		navAbner 				= objAbner.GetComponent<NavMeshAgent> ();

		objRoyo					= GameObject.Find ("Captain Aiden Royo");
		royoInteractable 		= objRoyo.GetComponent<InteractableRev>();
		navRoyo 				= objRoyo.GetComponent<NavMeshAgent> ();

		objElke					= GameObject.Find ("Elke Rassendyll");
		elkeInteractable		= objElke.GetComponent<InteractableRev>();
		navElke 				= objElke.GetComponent<NavMeshAgent> ();

		// ====================================================================

		conversationUI 			= GameObject.Find("Conversation").GetComponent<ConversationUI>();

		actionLine 				= GameObject.Find ("sayNeutral").GetComponent<TextMesh> ();
		actionLineShadow 		= GameObject.Find ("sayNeutralShadow").GetComponent<TextMesh> ();

		debugStatus 			= GameObject.Find ("DebugStatus").GetComponent<TextMesh> ();

		inv 					= GameObject.Find ("Inventory").GetComponent<Inventory> ();

		faderCard 				= GameObject.Find ("FaderCard");
		intro01					= GameObject.Find ("SlideshowIntro01");
		intro01SlideshowMats 	= intro01.GetComponent<SlideshowMaterials> ();

		iTween.CameraFadeAdd ();

		camFollow				= GetComponent<CameraFollow>();

	}
	
	// Update is called once per frame
	void Update () {
	
		deltaTimeModifier = Mathf.Lerp (deltaTimeModifier, dTimeTarget, dTimeTargetSpeed * Time.deltaTime);
		
		Mathf.Clamp (deltaTimeModifier, 0.0f, 1.0f);
		
		dTimeModified = Time.deltaTime * deltaTimeModifier;

		if (Input.GetKeyDown(KeyCode.RightBracket) && actionLine != null){ // Rev: Increases reading speed, prints status on action line
			readingSpeed += 0.5f;
			readingSpeed = Mathf.Clamp(readingSpeed, 0.5f, 8.0f);
			actionLine.text = "ReadingSpeed: " + readingSpeed;
			actionLineTime = 0.0f;
			actionLineReset = true;
		}

		if (Input.GetKeyDown(KeyCode.LeftBracket) && actionLine != null){ // Rev: Increases reading speed, prints status on action line.
			readingSpeed -= 0.5f;
			readingSpeed = Mathf.Clamp(readingSpeed, 0.5f, 8.0f);
			actionLine.text = "ReadingSpeed: " + readingSpeed;
			actionLineTime = 0.0f;
			actionLineReset = true;
		}

		if(actionLineTime < GameFlow.instance.readingSpeed){ // Rev: Timer to reset action line.
			actionLineTime += Time.deltaTime;
		}
		
		if(actionLineTime >= GameFlow.instance.readingSpeed && actionLineReset){
			hideActionLine();
			actionLineReset = false;
		}

		if(Input.GetKeyDown (KeyCode.KeypadEnter)){
			if(debugStatusVisible){
				debugStatus.text = null;
				debugStatusVisible = false;
			}else if (!debugStatusVisible){
				// Debug.Log("Should be displaying Status...");
				debugStatusSetup();
				debugStatusVisible = true;
			}
		}

		if(Input.GetKeyDown (KeyCode.Keypad1))CutsceneIntro();
		if(Input.GetKeyDown (KeyCode.Keypad2))EavesdropSetup();
		if(Input.GetKeyDown (KeyCode.Keypad3))CutsceneEnvelope();
		if(Input.GetKeyDown (KeyCode.Keypad4))SealedSetup();
		if(Input.GetKeyDown (KeyCode.Keypad5))CutsceneMortar();
		if(Input.GetKeyDown (KeyCode.Keypad6))WhiskeySetup();
		if(Input.GetKeyDown (KeyCode.Keypad7))ComputerSetup();
		if(Input.GetKeyDown (KeyCode.Keypad8))CutsceneConfront();
		if(Input.GetKeyDown (KeyCode.Keypad9))FinaleSetup();
		if(Input.GetKeyDown (KeyCode.Keypad0))CutsceneOutro();
		if(Input.GetKeyDown (KeyCode.KeypadPeriod))CutsceneCredits();

//		if(Input.GetKeyDown (KeyCode.KeypadDivide)){
//			Debug.Log ("The vector is: " + parseVector3("001.81,002.71,003.61"));
//		}
	}

	private void hideActionLine(){
		if(actionLine == null) return;
		actionLine.text = "";
		actionLineShadow.text = "";
	}

	public void PlayAudioGeneric (int clipNum){
		GetComponent<AudioSource>().clip = audClips [clipNum];
		GetComponent<AudioSource>().Play ();
	}

	/**
	 * Play the given animation, and do whatever we need to whenever we play an animation
	 */
	public void playAnimation(string animationName){
		GetComponent<Animation>().Play(animationName);
	}
	
	/**
	 * Super generic method, called whenever anything sends a broadcast with the name 'event'. Added here mainly to show
	 * how other scripts can subscribe to this broadcast if they need to. 
	 * 
	 * To send a broadcast use this line:
	 * 
	 * Messenger<string>.Broadcast("event", eventName);
	 * 
	 */
	private void HandleEvent(string eventName){
		Debug.Log("Detected event: " + eventName);

		if(eventName == "PlayerEmpty coffee cup and saucer" || eventName == "Empty coffee cup and saucerPlayer" || eventName == "PlayerCracked coffee cup" || eventName == "Cracked coffee cupPlayer"){
			PickUpEmptyCup();
		}

		if(eventName == "Coffee makerEmpty coffee cup and saucer" || eventName == "Empty coffee cup and saucerCoffee maker" || eventName == "Coffee makerHot cup of coffee with saucer" || eventName == "Hot cup of coffee with saucerCoffee maker"){
			// Making coffee to distract Royo
			if(preEavesdropNode.gameObject.activeSelf){
				GetComponent<Animation>().Play("DistractRoyo");
			}

			// Just making coffee
			else {
				MakingCoffee();
			}
			
		}

		if(eventName == "Coffee makerCracked coffee cup" || eventName == "Cracked coffee cupCoffee maker"){
			playAnimation("MakeCrackedCoffee");
		}

		if(eventName == "Sealed envelopeCoffee maker"){
			playAnimation("OpeningTheEnvelope");
			Debug.Log ("Opening the envelope...");
		}

		if(eventName == "Hot cup of coffee with saucerCaptain Aiden Royo"){
			GameObject cup = GameObject.Instantiate(spawnEmptyCup) as GameObject;
			cup.name = cup.name.Replace ("(Clone)","");
		}

		if(eventName == "RoyoCoffeeSpill"){
			// KT: After coffee spill conversation
			Debug.Log ("Spilling coffee...");
			playAnimation("SpillCoffeeOnRoyo");
			objRoyo.GetComponent<Collider>().enabled = false; // KT: we don't need to talk to Royo, but need to be able to click the orders in his pocket
		}

		if(eventName == "ForgeryPuzzle"){
			// KT: Give player the forged orders
			addItem(forgedOrders);
			GetComponent<Animation>().Play("ForgingDocuments");
			objRoyo.GetComponent<Collider>().enabled = true; // KT: And now we need to talk to him again
		}


		if(eventName == "RoyoForgedOrders"){
			// KT: Royo has received the forged orders
			Debug.Log ("Royo has orders and leaves! Boom?");
		}

		if(eventName == "TumblerDoor"){
			GetComponent<Animation>().Play("ListenDoor");
		}

	}

	// ====================================================================
	// Rev: Cutscene functions for Player
	public void playerSay(string text){
		playerInteractable.say(text);
		//sayChar.text = text;
		//sayCharShadow.text = text;
		//ResetReadingTime();
	}

	public void playerGo(string destination){
		navMaja.SetDestination (parseVector3(destination));
	}

	public void playerTurn(string destination){

		Vector3 tempDest = parseVector3 (destination);
		navMaja.updateRotation = false; // Rev: Turn off the NavMeshAgent's rotation...
		Vector3 relativePos = tempDest - objMaja.transform.position; // Rev: ...get vector between current pos and target...
		objMaja.transform.rotation = Quaternion.LookRotation (relativePos); // Rev: ...apply rotation...
		navMaja.updateRotation = true; // Rev: ...and turn the NavMeshAgent rotation back on!
	}

	public void playerTeleport(string destination){
		Vector3 tempDest = parseVector3 (destination);
		navMaja.updatePosition = false;
		objMaja.transform.position = tempDest;
		navMaja.updatePosition = true;
	}

	// ====================================================================
	// Rev: Cutscene functions for Abner
	public void abnerSay(string text){
		abnerInteractable.say (text);
	}

	public void abnerGo(string destination){
		navAbner.SetDestination (parseVector3(destination));
	}

	public void abnerTurn(string destination){
		
		Vector3 tempDest = parseVector3 (destination);
		navAbner.updateRotation = false; // Rev: Turn off the NavMeshAgent's rotation...
		Vector3 relativePos = tempDest - objAbner.transform.position; // Rev: ...get vector between current pos and target...
		objAbner.transform.rotation = Quaternion.LookRotation (relativePos); // Rev: ...apply rotation...
		navAbner.updateRotation = true; // Rev: ...and turn the NavMeshAgent rotation back on!
	}

	public void abnerTeleport(string destination){
		Vector3 tempDest = parseVector3 (destination);
		navAbner.updatePosition = false;
		objAbner.transform.position = tempDest;
		navAbner.updatePosition = true;
	}

	public void abnerPat(){
		abnerInteractable.abnerPat ();
	}

	// ====================================================================
	// Rev: Cutscene functions for Royo
	public void royoSay(string text){
		// KT: This is a rather hacky way of doing this, is it a good idea?
		// royoInteractable = GameObject.Find ("Captain Aiden Royo").GetComponent<InteractableRev>();
		royoInteractable.say(text);
	}

	public void royoGo(string destination){
		navRoyo.SetDestination (parseVector3(destination));
	}

	public void royoTurn(string destination){
		
		Vector3 tempDest = parseVector3 (destination);
		navRoyo.updateRotation = false; // Rev: Turn off the NavMeshAgent's rotation...
		Vector3 relativePos = tempDest - objRoyo.transform.position; // Rev: ...get vector between current pos and target...
		objRoyo.transform.rotation = Quaternion.LookRotation (relativePos); // Rev: ...apply rotation...
		navRoyo.updateRotation = true; // Rev: ...and turn the NavMeshAgent rotation back on!
	}

	public void royoTeleport(string destination){
		Vector3 tempDest = parseVector3 (destination);
		navRoyo.updatePosition = false;
		objRoyo.transform.position = tempDest;
		navRoyo.updatePosition = true;
	}

	public void royoIsBrushing(){
		royoInteractable.royoIsBrushing ();
	}

	public void royoNotBrushing(){
		royoInteractable.royoNotBrushing ();
	}

	public void royoRemoveConversation(string conversationName){
		royoInteractable.removeEventWithConvoName(conversationName);
	}

	// ====================================================================
	// Rev: Cutscene functions for Elke
	public void elkeSay(string text){
		elkeInteractable.say (text);
	}

	public void elkeGo(string destination){
		navElke.SetDestination (parseVector3(destination));
	}

	public void elkeTurn(string destination){
		
		Vector3 tempDest = parseVector3 (destination);
		navElke.updateRotation = false; // Rev: Turn off the NavMeshAgent's rotation...
		Vector3 relativePos = tempDest - objElke.transform.position; // Rev: ...get vector between current pos and target...
		objElke.transform.rotation = Quaternion.LookRotation (relativePos); // Rev: ...apply rotation...
		navElke.updateRotation = true; // Rev: ...and turn the NavMeshAgent rotation back on!
	}

	public void elkeTeleport(string destination){
		Vector3 tempDest = parseVector3 (destination);
		navElke.updatePosition = false;
		objElke.transform.position = tempDest;
		navElke.updatePosition = true;
	}

	public void elkeSitDown(){
		elkeInteractable.elkeSitDown();
	}
	
	public void elkeStandUp(){
		elkeInteractable.elkeStandUp();
	}
	
	public void elkeChop(){
		elkeInteractable.elkeChop ();
	}
	
	public void elkePound(){
		elkeInteractable.elkePound();
	}
	
	public void elkeThrow(){
		elkeInteractable.elkeThrow ();
	}

	public void elkeBobble(){
		elkeInteractable.headBobble ();
	}

	// ====================================================================

	public void addItem(GameObject item){
		GameObject newitem = GameObject.Instantiate (item) as GameObject;
		newitem.name = newitem.name.Replace ("(Clone)", "");
		inv.addItem (newitem);
	}

	// Rev: Function that can be triggered by animation clip
	private bool _clickThrough; // KT: super quick way of going to auto text and back in cutscenes
	public void PauseInput(int isTrue){
		if (isTrue == 0){
			conversationClickThrough = _clickThrough;
			inputPaused = false;
		} else if (isTrue == 1){
			_clickThrough = conversationClickThrough;
			conversationClickThrough = false;
			inputPaused = true;
		}
	}

	public void ToggleCameraTrack(){
		camFollow.trackPlayer = !camFollow.trackPlayer;
		}

//	public void RoyoTurns(){
//		//Debug.Log ("Royo turns around after spilling coffee on himself");
//		royoInteractable.gameObject.transform.Rotate(Vector3.up, 180);
//	}
//
//	public void RoyoTurnsBack(){
//		royoInteractable.gameObject.transform.Rotate(Vector3.up, 180);
//	}

	// Rev: Following is a list of functions for setting up minor repeating events in the game.

	public void MakingCoffee() {
		playAnimation("MakeCoffee");
	}

	public void PickUpEmptyCup(){
		GetComponent<AudioSource>().clip = audClips [2];
		GetComponent<AudioSource>().Play ();
	}

	// Rev: Functions for the introduction

	public void IntroSlideVisible(int isVisible){

		if(isVisible > 0){
			intro01.GetComponent<Renderer>().enabled = true;
		}else{
			intro01.GetComponent<Renderer>().enabled = false;
		}
	}

	public void IntroSlideChange(int slideNumber){
		intro01SlideshowMats.SwitchSlide (slideNumber);
	}


	// Rev: Following is a list of functions for setting up MAJOR events and cutscenes in the game.

	public void CutsceneIntro(){
		// playAnimation ("GameIntro");
		// playAnimation ("ElkeSitTest");
		playAnimation ("Intro");
		cutsceneIntro = true;
		debugStatusSetup();
	}

	public void EavesdropSetup(){
		eavesdropSetup = true;
		debugStatusSetup();

		preEavesdropNode.gameObject.SetActive(false);
		eavesdropNode.gameObject.SetActive(true);
		objRoyo.GetComponent<RoyoBlock>().enabled = false;
	}

	public void ForgerySetup(){
		eavesdropNode.gameObject.SetActive(false);
		forgeryNode.gameObject.SetActive(true);
	}

	public void ForgerySetup2(){
		forgeryNode.gameObject.SetActive(false);
		forgeryNode2.gameObject.SetActive(true);
	}

	public void PreBoomSetup(){
		forgeryNode2.gameObject.SetActive(false);
		preBoomNode.gameObject.SetActive(true);
	}

	public void CutsceneEnvelope(){
		cutsceneEnvelope = true;
		debugStatusSetup();
	}

	public void SealedSetup(){
		GameObject newitem = GameObject.Instantiate (testInvItem) as GameObject;
		newitem.name = newitem.name.Replace ("(Clone)", "");
		inv.addItem (newitem);
		sealedSetup = true;
		debugStatusSetup();
	}

	public void CutsceneMortar(){
		cutsceneMortar = true;
		debugStatusSetup();
	}

	public void WhiskeySetup(){
		whiskeySetup = true;
		debugStatusSetup();
	}

	public void ComputerSetup(){
		computerSetup = true;
		debugStatusSetup();
	}

	public void CutsceneConfront(){
		cutsceneConfront = true;
		debugStatusSetup();
	}

	public void FinaleSetup(){
		finaleSetup = true;
		debugStatusSetup();
	}

	public void CutsceneOutro(){
		cutsceneOutro = true;
		debugStatusSetup();
	}

	public void CutsceneCredits(){
		cutsceneCredits = true;
		debugStatusSetup();
	}

	public void FadeToBlack (float time = 1.0f){
		iTween.FadeTo (faderCard, iTween.Hash ("amount", 1.0, "time", time, "easeType", "easeOutQuart"));
	}

	public void FadeToClear (float time = 1.0f){
		iTween.FadeTo (faderCard, iTween.Hash ("amount", 0.0, "time", time, "easeType", "easeOutQuart"));
	}
	
	public void debugStatusSetup () { // Rev: This updates the debug 3DText with the state bools set by the event functions.
		debugStatus.text = 	"1 Cutscene Intro: " + cutsceneIntro + "\n" +
							"2 Eavesdrop Setup: " + eavesdropSetup + "\n" +
							"3 Cutscene Envelope: " + cutsceneEnvelope + "\n" +
							"4 Sealed Setup: " + sealedSetup + "\n" +
							"5 Cutscene Mortar: " + cutsceneMortar + "\n" +
							"6 Whiskey Setup: " + whiskeySetup + "\n" +
							"7 Computer Setup: " + computerSetup + "\n" +
							"8 Cutscene Confront: " + cutsceneConfront + "\n" +
							"9 Finale Setup: " + finaleSetup + "\n" +
							"0 Cutscene Outro: " + cutsceneOutro + "\n" +
							". Cutscene Credits: " + cutsceneCredits;
	}

	// Rev: Convert a string (eg, "001,023,042") into a Vector3 (001,023,042) - for use moving characters around with cutscenes.
	// Rev: ...because the bloody Anim Events won't take a Gameobject in-scene or a Vector3)
	private Vector3 parseVector3(string sourceString){

		// string 		outString;
		Vector3 	outVector3;
		string[]	splitString;

		// Rev: Trim extraneous parenthesis (Do we need this?)
		//outString = sourceString.Substring (1, sourceString.Length - 2);

		// Rev: Split delimited values into an array
//		splitString = outString.Split ("," [0]);
		splitString = sourceString.Split ("," [0]);

		// Rev: Construct new V3 from array elements
		float x = float.Parse (splitString [0]);
		float y = float.Parse (splitString [1]);
		float z = float.Parse (splitString [2]);

		outVector3 = new Vector3 (x, y, z);

		return outVector3;
	}


}
