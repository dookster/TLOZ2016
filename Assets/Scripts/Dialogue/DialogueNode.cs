using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//using UnityEditor; // Disable this before building final game

/**
 * Represents a single connection in a conversation. 
 * 
 * The name of the gameobject is the sentence the player can choose to say, the variable
 * 'response' is what the character who owns the conversation will reply. 
 * 
 * All subsequent dialogue options are children of this object's transform.
 * 
 * !! NB !! Uncomment the OnDrawGizmos method and the 'using UnityEditor' lines before building game.
 * 
 */
[ExecuteInEditMode]
public class DialogueNode : MonoBehaviour {

	public string playerLine;
	public string response;

	private GUIStyle style;
	private GUIStyle style2;
	 	
	private List<DialogueNode> children = new List<DialogueNode>();
	public DialogueNode parentNode;

	// Use this for initialization
	void Start () {
		// Find style in parents
		style = GetComponentInParent<DialogueConversation>().editorStyle;
		style2 = GetComponentInParent<DialogueConversation>().editorStyle2;
		
		loadChildren();

		response = response.Replace ("NUULINE", "\n");
	}

	public void loadChildren(){
		children.Clear();

		// Get a reference to the parent if it's a DialogueNode
		if(transform.parent != null){
			parentNode = transform.parent.GetComponent<DialogueNode>();
		}
		
		// Get a list of all children
		for(int n = 0 ; n < transform.childCount ; n++){
			children.Add(transform.GetChild(n).GetComponent<DialogueNode>());
		}
	}
	

	// Update is called once per frame
	void Update () {
	
	}

	public GUIStyle getGizStyle1(){
		return style;
	}

	public GUIStyle getGizStyle2(){
		return style2;
	}

	public void setGizStyle1(GUIStyle gizStyle){
		style = gizStyle;
	}

	public void setGizStyle2(GUIStyle gizStyle){
		style2 = gizStyle;
	}

	public List<DialogueNode> getChildren(){
		return children;
	}
	

}

