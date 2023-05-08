using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BrennanHatton.GPT;
using OpenAI.Integrations.VoiceRecorder;
//Teams
//Players
//Judge
//Roles
//Backstory / Case

[System.Serializable]
public class Player
{
	public string name;
	public Role role;
	public Party party;
	public List<Statement> statements = new List<Statement>();
}

	[System.Serializable]
public class Statement
{
	public string context, repsonse;
	public Player player;
}

[System.Serializable]
public class Role
{
	public string name,
		backstory;
}

[System.Serializable]
public class Trail
{
	public string name,
		accusation,
		details;
		
	public bool ready = false;
		
	public GPT3 gpt;
		
	public void SetTrail(GPT3 _gpt)
	{
		InteractionEvent callback = new InteractionEvent();
		callback.AddListener(SetAccusation);
		
		gpt = _gpt;
		gpt.Execute("Come up with an abusrd or hilarious accursation to be argued in court between two parties.",
			callback);
	}
	
	void SetAccusation(InteractionData data)
	{ 
		Debug.Log("SetAccusation");
		accusation = data.generatedText; 
		
		InteractionEvent callback = new InteractionEvent();
		callback.AddListener(NameCase);
		gpt.Execute("Come up with a name for this case:" + data.generatedText,
			callback);
	}
	
	void NameCase(InteractionData data)
	{
		Debug.Log("NameCase");
		name = data.generatedText;
		
		ready = true;
	}
}

[System.Serializable]
public class Party
{
	public string name;
	public List<Player> players = new List<Player>();
}

[System.Serializable]
public class Judge
{
	public List<Statement> statements;
	public Trail trail;
	public string name;
	//public Voices voice;
}

public class GameManager : MonoBehaviour
{
	public Party[] parties;
	public List<Player> players;
	public Trail trail;
	public Judge judge;
	public GPT3 Gpt;
	public ELSpeaker JudgeVoiceSpeaker;
	
	public void NewGame()
	{
		parties = new Party[2] {new Party(), new Party()};
		
	}
	
	public void AddPlayer()
	{
		Player newPlayer = new Player();
		players.Add(newPlayer);
		
		//assign party
		newPlayer.party = parties[Convert.ToInt32(parties[0].players.Count > parties[1].players.Count)];
	}
	
	public void RemovePlayer(Player playerToRemove)
	{
		playerToRemove.party.players.Remove(playerToRemove);
		players.Remove(playerToRemove);
	}
	
	public void NewCase()
	{
		
		trail = new Trail();
		trail.SetTrail(Gpt);
		
		judge = new Judge();
		judge.trail = trail;
		judge.name = "Judge Gina Patric Terry";
	}
	
    // Start is called before the first frame update
    void Start()
    {
	    NewGame();
	    
	    AddPlayer();
	    AddPlayer();
		
	    NewCase();
	    
	    StartCoroutine(_startWhenReady());
	    
    }
    
	IEnumerator _startWhenReady()
	{
		while(!trail.ready)
			yield return new WaitForSeconds(0.5f);
			
		StartGame();
	}
    
	public void StartGame()
	{
		Debug.Log("StartGame");
		//Read out trail.
		JudgeVoiceSpeaker.SpeakSentence(trail.accusation);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
