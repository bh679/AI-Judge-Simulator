using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptManager : MonoBehaviour
{
	public string 
		newCase = "Come up with an abusrd or hilarious accursation to be argued in court between two parties.",
		nameCase = "Come up with a name for this case:",
		announceNewGame = "You are a judge in a televised small claims court TV show. Announce a new episode has started",
		newBackstory = "Come up with a backstory for # the ^ in case where $";
	
	
	public static PromptManager Instance { get; private set; }
	private void Awake() 
	{ 
		// If there is an instance, and it's not me, delete myself.
    
		if (Instance != null && Instance != this) 
		{ 
			Destroy(this); 
		} 
		else 
		{ 
			Instance = this; 
		} 
	}
}
