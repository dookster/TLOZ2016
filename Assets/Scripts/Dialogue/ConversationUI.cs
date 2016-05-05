using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConversationUI : MonoBehaviour {

	public Camera uiCamera;
	public GameObject optionTextPrefab;

	private List<OptionText> UILines = new List<OptionText>();

	private DialogueConversation currentConversation;

	// Use this for initialization
	void Start () {
		uiCamera = GameObject.Find("actionLineCamera").GetComponent<Camera>() ;
	}
	
	// Update is called once per frame
	void Update () {

		// Handle keyboard input
		if(currentConversation != null){
//			if(Input.GetKeyUp("1")){
//				currentConversation.selectConversationOption(0);
//				hideOptions();
//			}
//			if(Input.GetKeyUp("2")){
//				currentConversation.selectConversationOption(1);
//				hideOptions();
//			}
//			if(Input.GetKeyUp("3")){
//				currentConversation.selectConversationOption(2);
//				hideOptions();
//			}
		}


		// Mouse input

		Ray ray = uiCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		// All this can be handled better!

		if(Physics.Raycast(ray, out hit)){									// Rev: If the raycast hits something...
			if (hit.transform.name == "option") {							// ...and the object has a transform named 'option'...
				if(Input.GetButtonUp("Fire1")){
					for(int n = 0 ; n < UILines.Count ; n++){ 					// ... go through the UILines array...
						if(UILines[n].transform.Equals(hit.transform)){ 		// ...and if the current inspected textmesh's transform in the array is equal to the current hit...
							currentConversation.selectConversationOption(n); 	// ...trigger the Select function of the current conversation with the selected textmesh.
							hideOptions();
						}
					}
				}
			}
		}
	}

	public void showOptions(DialogueConversation conversation){				// Rev: Make the parameter the current conversation, cache a reference to the current conv. DialogueNode
		currentConversation = conversation;
		DialogueNode currentNode = conversation.currentNode;
		int optionCount = currentNode.getChildren().Count;

		// KT: Create all the lines we need (once created we'll only create more if we have more options than last time)
		for(int c = optionCount - UILines.Count; c > 0 ; c--){				
			GameObject ob = Instantiate(optionTextPrefab) as GameObject;
			ob.transform.parent = transform;
			ob.transform.name = "option";
			OptionText uiLine = ob.GetComponent<OptionText>();
			UILines.Add(uiLine);
		}
		if(optionCount > 0){												// Rev: If currentNode has children, iterate through and set each textmesh to position and SetActive.
			for(int n = 0 ; n < optionCount ; n++){							// Rev: Each textmesh text content is set to the name of the node
			//foreach(DialogueNode node in currentNode.children){
				DialogueNode node = currentNode.getChildren()[n];
				OptionText line = UILines[n];
				line.gameObject.SetActive(true);
				line.setText("* " + node.name);
				line.transform.localPosition = Vector3.zero;
				line.transform.Translate(0, 0.6f * n, 0);
			}
		}
	}

	public void hideOptions(){												// Rev: Iterate through current UILines, disable gameobject
		for(int n = 0 ; n < UILines.Count ; n++){
			OptionText line = UILines[n];
			line.gameObject.SetActive(false);
		}
	}
}
