using UnityEngine;
using System.Collections;
using UnityEditor;

public class DialogueEditorGizmo : MonoBehaviour {


	public GUIStyle editorStyle;
	private DialogueNode[] nodes;

	// Use this for initialization
	void Start () {
		 nodes = GameObject.FindObjectsOfType<DialogueNode>();
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log("WEEE");
		Handles.BeginGUI();
		//if(editorStyle == null) editorStyle = GetComponentInParent<DialogueConversation>().editorStyle;
		foreach(DialogueNode node in nodes){
			Vector3 labelPos = new Vector3(node.transform.position.x - node.transform.localScale.x/4, node.transform.position.y + node.transform.localScale.y / 4, node.transform.position.z);
			Handles.Label(labelPos, " - " + node.gameObject.name, editorStyle);
			Handles.Label(labelPos, "\n"+ "Response: " + node.response);
		}
		Handles.EndGUI();
	}

	void OnDrawGizmos() {
	
	}
}
