using UnityEngine;
using System.Collections;

/**
 *  Base class for small scripts listening in on event broadcast. Extend and add
 *  to anything that needs to appear/disappear/move/change when something specific happens.
 */
public abstract class StoryElement : MonoBehaviour {

	void OnEnable(){
		// Listen for any broadcasts of the type 'event'
		Messenger<string>.AddListener("event", HandleEvent);
	}
	
	void OnDestroy(){
		Messenger<string>.RemoveListener("event", HandleEvent);
	}

	public abstract void HandleEvent(string eventName);
}
