//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BrennanHatton.GPT;
using OpenAI.Integrations.VoiceRecorder;

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
	
	public string StatementSummary()
	{
		string summary = role.name + " " + name;
		
		summary += " has made the following statements:\n";
		
		for(int i = 0; i < statements.Count; i++)
		{
			summary += statements[i].repsonse+"\n";
		}
		
		summary += "end of statements for " + role.name + " " +  name;
		
		return summary;
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
		JuryMember = new Role("Jury Member", Party.Neutral),
		Audience = new Role("Audience Member", Party.Either),
		Press = new Role("Member of the Press", Party.Neutral);

	public static Role[] roles = new Role[8] {
		Plaintiff, Defendant, Victim, Perpetrator, Witness, Audience, JuryMember, Press
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
		judge = new Judge();
		judge.name = "Judge Gina Patric Terry";
		
		InteractionEvent callback = new InteractionEvent();
		callback.AddListener((InteractionData)=>{
			
			judge.name = InteractionData.generatedText;
			GenerateAndSpeak(PromptManager.Instance.announceNewGame.Replace("#","Judge "+judge.name));
		});
		
		Gpt.Execute(PromptManager.Instance.newJudeName,
			callback);
		
		
		
			
		roles = new List<Role>();
		for(int i =0 ; i < Role.roles.Length; i++)
		{
			roles.Add(Role.roles[i]);
		}
		
	}
	
	public void AddPlayer(string name)
	{
		Player newPlayer = new Player(Gpt);
		players.Add(newPlayer);
		
		newPlayer.name = name;
		newPlayer.SetRole(GetRandomRole());
		
		GenerateAndSpeak(PromptManager.Instance.announceNewPlayer.Replace("#", newPlayer.name) + " as a/the " + newPlayer.role.name);
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
		
		judge.trial = trial;
		
	}
    
	void SetBackstories(InteractionData data)
	{
		for(int i = 0 ; i < players.Count; i++)
		{
			players[i].SetTrial(trial);
		}
	}
	
	//cut off, skip or queue, jump queue
	void GenerateAndSpeak(string prompt)
	{
		InteractionEvent callback = new InteractionEvent();
		callback.AddListener(VoiceResponse);
		
		Gpt.Execute(prompt,
			callback);
	}
	
	void VoiceResponse(InteractionData data)
	{
		if(!JudgeVoiceSpeaker.GetComponent<AudioSource>().isPlaying)
			JudgeVoiceSpeaker.SpeakSentence(data.generatedText);
	}
	
    // Start is called before the first frame update
    void Start()
    {
	    NewGame();
	    
	    StartCoroutine(_startWhenReady());
	    
    }
    
	IEnumerator _startWhenReady()
	{
		
		yield return new WaitForSeconds(15.5f);
		while(JudgeVoiceSpeaker.GetComponent<AudioSource>().isPlaying)
			yield return new WaitForSeconds(0.5f);
			
		AddPlayer("Faith");
		
		yield return new WaitForSeconds(15.5f);
		while(JudgeVoiceSpeaker.GetComponent<AudioSource>().isPlaying)
			yield return new WaitForSeconds(0.5f);
			
		AddPlayer("Brennan");
		
		yield return new WaitForSeconds(15.5f);
		while(JudgeVoiceSpeaker.GetComponent<AudioSource>().isPlaying)
			yield return new WaitForSeconds(0.5f);
		
		NewCase();
		
		while(!trial.ready)
			yield return new WaitForSeconds(0.5f);
			
		StartGame();
		
		
		while(playersTurn < players.Count)
		{
			yield return new WaitForSeconds(15.5f);
			while(JudgeVoiceSpeaker.GetComponent<AudioSource>().isPlaying)
				yield return new WaitForSeconds(0.5f);
	
	
			NextPlayer();
		}
		
		DrawConclusion();
	}
    
	public void StartGame()
	{
		Debug.Log("StartGame");
		
		//Read out trail.
		JudgeVoiceSpeaker.SpeakSentence(trial.accusation);
		
	}
	
	int playersTurn = 0;
	public void NextPlayer()
	{
		Player currentPlayer = players[playersTurn];
		GenerateAndSpeak(PromptManager.Instance.announcePlayersTurn.Replace("#",currentPlayer.name + " " + currentPlayer.role.name).Replace("%",trial.name).Replace("^",currentPlayer.backstory));
		
		playersTurn++;
	}
	
	public void DrawConclusion()
	{
		string playerDetails = "";
		
		for(int i = 0; i < players.Count; i++)
		{
			playerDetails += players[i].StatementSummary();
		}
		
		GenerateAndSpeak(
			PromptManager.Instance.drawConclusion.Replace("#",judge.name) + trial.details +"\n\n" + playerDetails
		);
	}
	
	
}
