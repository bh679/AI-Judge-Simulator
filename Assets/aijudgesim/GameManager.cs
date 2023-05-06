using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BrennanHatton.GPT;

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
	public Party team;
	public List<Statement> statements;
}

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
}

[System.Serializable]
public class Party
{
	public string name;
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
	public TextAsset judgeNames; 
	
	public void NewCase()
	{
		parties = new Party[2];
		
		trail = new Trail();
		trail.accusation = "Come up with an accursation to be argued in court between two parties";
		trail.name = "Name this case";
		trail.details = "Details of this case";
		
		judge = new Judge();
		judge.trail = trail;
		judge.name = "";
	}
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
