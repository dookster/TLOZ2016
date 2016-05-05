using UnityEngine;
using System.Collections;

public class OptionText : MonoBehaviour {

	public TextMesh mainText;
	public TextMesh shadowText;

	public Color	neutral;
	public Color	highlight;
	public Color	shadow;

	// Use this for initialization
	void Start () {
	
		mainText.color = neutral;
		shadowText.color = shadow;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseEnter(){
		// Highlight the text when we mouse over
		mainText.color = new Color(highlight.r, highlight.g, highlight.b, 1.0f);
		shadowText.color = shadow;
//		shadowText.color = new Color(shadowText.color.r, shadowText.color.g, shadowText.color.b, 0.5f);
	}

	void OnMouseExit(){
		mainText.color = new Color(neutral.r, neutral.g, neutral.b, 1.0f);
		shadowText.color = shadow;
//		shadowText.color = new Color(shadowText.color.r, shadowText.color.g, shadowText.color.b, 1);
	}

	public void setText(string t){
		mainText.text = t;
		shadowText.text = t;

		// Reset color
		mainText.color = new Color(mainText.color.r, mainText.color.g, mainText.color.b, 1);
		shadowText.color = shadow;
//		shadowText.color = new Color(shadowText.color.r, shadowText.color.g, shadowText.color.b, 1);
	}

}
