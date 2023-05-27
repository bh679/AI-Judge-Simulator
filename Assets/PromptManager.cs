using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptManager : MonoBehaviour
{
	public string 
		newCase = "Come up with an abusrd or hilarious accursation to be argued in court between two parties.",
		nameCase = "Come up with a name for this case:",
		newBackstory = "Come up with a backstory for # the ^ in case where $",
		newJudeName = "Come up with the name of a judge whos initials are GPT";
		
	public string 
		announceNewGame = "You are # in a televised small claims court TV show. Announce a new episode has started",
		announceNewPlayer = "Announce # entering the room.",
		announcePlayersTurn = "Announce #'s time to speak in the courtroom, and ask what they have to say";
		
	public string
		drawConclusion = "You are # in a televised small claims court TV show. You are drawing your conclusion and announcing the verdict based on the following evidence.";
	
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
