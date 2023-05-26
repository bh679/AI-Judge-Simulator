//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BrennanHatton.GPT;
using OpenAI.Integrations.VoiceRecorder;
//Teams
//Players
//Judge
//Roles
//Backstory / Case

/*public enum Role
{
	Plantive = 0, // prosecutor
	Defendant = 1,// despodant
	Victim = 2,
	Perpetrator = 3, // Accusted 
	Witnesses = 4,
	Judge = 5,
	JuryMember = 6,
	Audience = 7, // Gallery
	Press = 8 // Jounro - Real tweet button
	//SketchArtist// - MJ - Stable Diffusion
}*/



[System.Serializable]
public class Player
{
	public string name;
	public Role role;
	public List<Statement> statements = new List<Statement>();
	public string backstory;
	GPT3 gpt;
	
	public Player(GPT3 gpt)
	{
		this.gpt = gpt;
	}
	
	public void SetRole(Role role)
	{
		this.role = role;
	}
	
	public void SetTrial(Trial trial)
	{
		InteractionEvent callback = new InteractionEvent();
		callback.AddListener(SetBackstory);
		
		gpt.Execute(PromptManager.Instance.newBackstory.Replace("#",name).Replace("$",trial.accusation).Replace("^",role.name),
			callback);
	}
	
	void SetBackstory(InteractionData data)
	{ 
		Debug.Log("SetBackstory");
		backstory = data.generatedText; 
	}
}

[System.Serializable]
public class Role
{
	public string name;
	public Party party;

	public Role(string name, Party party)
	{
		this.name = name;
		this.party = party;
	}

	public static Role Plaintiff = new Role("Plaintiff", Party.Plaintiff),
		Defendant = new Role("Defendant", Party.Defendant),
		Victim = new Role("Victim", Party.Defendant),
		Perpetrator = new Role("Perpetrator", Party.Plaintiff),
		Witness = new Role("Witness", Party.Either),
		Judge = new Role("Judge", Party.Neutral),
		JuryMember = new Role("JuryMember", Party.Neutral),
		Audience = new Role("Audience", Party.Either),
		Press = new Role("Press", Party.Neutral);

	public static Role[] roles = new Role[9] {
		Plaintiff, Defendant, Victim, Perpetrator, Witness, Judge, Audience, JuryMember, Press
	};
}


[System.Serializable]
public class Statement
{
	public string context, repsonse;
	public Player player;
}

[System.Serializable]
public class Trial
{
	public string name,
		accusation,
		details;
		
	public bool ready = false;
		
	public GPT3 gpt;
		
	public void SetTrial(GPT3 _gpt, UnityAction<InteractionData> GPTCompleteAction = null)
	{
		InteractionEvent callback = new InteractionEvent();
		callback.AddListener(SetAccusation);
		if(GPTCompleteAction != null)
			callback.AddListener(GPTCompleteAction);
		
		gpt = _gpt;
		gpt.Execute(PromptManager.Instance.newCase,
			callback);
	}
	
	void SetAccusation(InteractionData data)
	{ 
		Debug.Log("SetAccusation");
		accusation = data.generatedText; 
		
		InteractionEvent callback = new InteractionEvent();
		callback.AddListener(NameCase);
		gpt.Execute(PromptManager.Instance.nameCase + data.generatedText,
			callback);
	}
	
	void NameCase(InteractionData data)
	{
		Debug.Log("NameCase");
		name = data.generatedText;
		
		ready = true;
	}
}


public enum Party
{
	Plaintiff = 0,
	Defendant = 1,
	Neutral = 2,
	Either = 3
}
	
/*[System.Serializable]
public class Party
{
	
	public Party side;
	public List<Player> players = new List<Player>();
	
	public Party(Party party)
	{
		this.side = party;
	}
}*/

[System.Serializable]
public class Judge
{
	public List<Statement> statements;
	public Trial trial;
	public string name;
	public bool ready = false;
	//public Voices voice;
}

public class GameManager : MonoBehaviour
{
	//public Party[] parties;
	public List<Player> players;
	public Trial trial;
	public Judge judge;
	public GPT3 Gpt;
	public ELSpeaker JudgeVoiceSpeaker;
	
	List<Role> roles; 
	
	public void NewGame()
	{
		//parties = new Party[3] {new Party(), new Party(""), new Party("")};
		
		InteractionEvent callback = new InteractionEvent();
		callback.AddListener(VoiceResponse);
		
		Gpt.Execute(PromptManager.Instance.announceNewGame,
			callback);
			
		roles = new List<Role>();
		for(int i =0 ; i < Role.roles.Length; i++)
		{
			roles.Add(Role.roles[i]);
		}
		
	}
	
	public void AddPlayer()
	{
		Player newPlayer = new Player(Gpt);
		players.Add(newPlayer);
		
		newPlayer.SetRole(GetRandomRole());
		//assign party
		//newPlayer.party = parties[Convert.ToInt32(parties[0].players.Count > parties[1].players.Count)];
	}
	
	Role GetRandomRole()
	{
		Role randRole = roles[Random.Range(0,roles.Count)];
		roles.Remove(randRole);
		
		return randRole;
	}
	
	public void RemovePlayer(Player playerToRemove)
	{
		//playerToRemove.party.players.Remove(playerToRemove);
		players.Remove(playerToRemove);
	}
	
	public void NewCase()
	{
		
		trial = new Trial();
		trial.SetTrial(Gpt, SetBackstories);
		
		judge = new Judge();
		judge.trial = trial;
		judge.name = "Judge Gina Patric Terry";
		
		
	}
    
	void SetBackstories(InteractionData data)
	{
		for(int i = 0 ; i < players.Count; i++)
		{
			players[i].SetTrial(trial);
		}
	}
	
	
	void VoiceResponse(InteractionData data)
	{
		if(!JudgeVoiceSpeaker.GetComponent<AudioSource>().isPlaying)
			JudgeVoiceSpeaker.SpeakSentence(trial.accusation);
	}
	
    // Start is called before the first frame update
    void Start()
    {
	    NewGame();
	    
	    AddPlayer();
	    AddPlayer();
		
	    NewCase();
	    
	    //StartCoroutine(_startWhenReady());
	    
    }
    
	IEnumerator _startWhenReady()
	{
		while(!trial.ready)
			yield return new WaitForSeconds(0.5f);
			
		StartGame();
	}
    
	public void StartGame()
	{
		Debug.Log("StartGame");
		//Read out trail.
		JudgeVoiceSpeaker.SpeakSentence(trial.accusation);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
