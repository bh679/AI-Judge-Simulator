using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SettingsManager))]
public class SettingsJsonGenerator : MonoBehaviour
{
	public string json;
	
	void Reset()
	{
		SettingsManager settings = this.GetComponent<SettingsManager>();
		
		json = JsonUtility.ToJson(settings.gameData.Data); 
	}
}
