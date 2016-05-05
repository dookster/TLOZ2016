using UnityEngine;
using System.Collections;

/**
 * Root node of a conversation
 */
public class DialogueConversation : DialogueNode{

	public GUIStyle editorStyle; // Gui style for gizmos in the editor
	public GUIStyle editorStyle2;

	public DialogueNode currentNode;
	private InteractableRev interactable; // The interactable 'owning' this conversation

	// Use this for initialization
	void Start () {
		currentNode = this;
		loadChildren();
		setGizStyle1(editorStyle);
		setGizStyle2(editorStyle2);

		response = response.Replace ("NUULINE", "\n"); // Rev: On start, this replaces the word NUULINE with an escape character which forces the following text onto a new line
	}
	
	// Update is called once per frame
	void Update () {

	}

	/**
	 * Select the conversation option with the given index
	 */
	public void selectConversationOption(int n){
		if(n < currentNode.getChildren().Count){
			currentNode = currentNode.getChildren()[n];
			talk ();
		}
	}

	/**
	 * Calling this will attempt to 'say' the player's line then the 
	 * NPC's line. Each time pausing for a moment before going on. If
	 * the current node has only one child we skip to this and call talk 
	 * again.
	 * 
	 * If any line is missing or empty ("") it is skipped, this way we
	 * can string together several lines for one character.
	 */
	private void talk(){
		StartCoroutine(sayPlayerLine());
	}

	private IEnumerator sayPlayerLine() {
		//Debug.Log ("name: " + currentNode.name + " line: " + currentNode.playerLine);
		if(currentNode.playerLine != null && currentNode.playerLine.Length > 0){
		//if(currentNode.name != null && currentNode.name.Length > 0){
			if(keyWordMatch(currentNode.playerLine)){
				// Current node is a keyword, don't say anything
				return true;
			}
			string tempString = currentNode.playerLine.Replace("NUULINE","\n");
			GameFlow.instance.playerSay(tempString);

			while(GameFlow.instance.playerInteractable.isTalking())
				yield return null;
		}

		StartCoroutine(sayNPCLine());
	}

	private IEnumerator sayNPCLine() {
		if(currentNode.response != null && currentNode.response.Length > 0){
			interactable.say(currentNode.response);

			while(interactable.isTalking())
				yield return null;
		}
		
		if(currentNode.getChildren().Count > 1){
			showOptions();
		} else {
			selectConversationOption(0);
		}
	}

	public void showOptions(){
		GameFlow.instance.conversationUI.showOptions(this);
	}

	public void startConversation(InteractableRev owner){
		// Perhaps we should place this somewhere else?
		GameFlow.instance.inputPaused = true;

		interactable = owner;
		currentNode = this;

		// Start with the response? Or perhaps always have a single node after the root?
		StartCoroutine(sayNPCLine());
	}

	/**
	 * Check if the given string matches any keyword (END, GOTO etc) and handle it.
	 * Return true if it was a match so the current conversation doesn't try to continue.
	 */
	private bool keyWordMatch(string key){
		//Debug.Log ("keyword: " + key);
		if(key.Equals("DELETE")){
			interactable.removeEventWithConvo(this);
			GameFlow.instance.inputPaused = false;
			return true;
		}
		if(key.Equals("END")){
			Debug.Log("Conversation END");
			GameFlow.instance.inputPaused = false;
			return true;
		}
		if(key.Equals("EVENT")){
			// Broadcast event with this conversation name and end
			Messenger<string>.Broadcast("event", (name));
			GameFlow.instance.inputPaused = false;
			return true;
		}
		if(key.Contains("GO UP")){
			// Go back X levels in the conversation, would be cool if we could skip to a specific one, this may do
			int levels = int.Parse(key.Substring(5));
			for(int n = 0 ; n < levels ; n++){
				currentNode = currentNode.parentNode;
			}
			talk();
		}
		return false;
	}

}
