using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class SettingsManager : MonoBehaviour
{
	//public Settings<SettingsData> settings;
	public RemoteData<GameData> gameData;
	
	public static SettingsManager Instance { get; private set; }
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
	
	void Start()
	{
		LoadFromFile();
	}
	
	public void LoadFromFile()
	{
		StartCoroutine(gameData.Load());
	}
	
	
	/*public void SaveToFile()
	{
		gameData.Save();
	}*/
}


[System.Serializable]
public class RemoteData<T>
{
	public T Data;
	public string filePath;
	public TextAsset textAsset;
	public DataType dataType = DataType.JSON;
	
	bool remoateLoaded = false;
	
	public IEnumerator Load()
	{
		
		if(!string.IsNullOrEmpty(filePath))
		{
			UnityWebRequest www = UnityWebRequest.Get(filePath);
			yield return www.SendWebRequest();
	
			if (www.result != UnityWebRequest.Result.Success) {
				Debug.Log(www.error);
			}
			else {
				// Show results as text
				Debug.Log(www.downloadHandler.text);
				
				if(dataType == DataType.JSON)
					Data = JsonUtility.FromJson<T>(www.downloadHandler.text);
	
				// Or retrieve results as binary data
				//byte[] results = www.downloadHandler.data;
					
				remoateLoaded = true;
			}
			
		}
		
		if(!remoateLoaded && textAsset != null)
		{
			Data = JsonUtility.FromJson<T>(textAsset.text);
		}
	}
	
	/*public void Save()
	{
		if(location == FileLocation.WWW)
		{
			Debug.LogError("Cant save file to remote");
			return;
		}
		
		File.WriteAllText(_filePath, JsonUtility.ToJson(Data));
	}*/
}

[System.Serializable]
public enum DataType
{
	JSON
}

[System.Serializable]
public class GameData
{
	public string[] judgeNames;
}