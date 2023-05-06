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
	
	public void LoadFromFile()
	{
		gameData.Load();
	}
	
	
	public void SaveToFile()
	{
		gameData.Save();
	}
}


[System.Serializable]
public class RemoteData<T>
{
	public T Data;
	public string filePath;
	public FileLocation location;
	public DataType dataType = DataType.JSON;
	string _filePath = null;
	
	public IEnumerator Load()
	{
		if(_filePath == null)
		{
			if (location == FileLocation.ApplicationData)
				_filePath = Path.Combine(Application.persistentDataPath, filePath);
		}
		
		if(location == FileLocation.WWW)
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
					Data = JsonUtility.FromJson<T>(File.ReadAllText(www.downloadHandler.text));
 
				// Or retrieve results as binary data
				//byte[] results = www.downloadHandler.data;
			}
		}
		
		if(dataType == DataType.JSON)
			Data = JsonUtility.FromJson<T>(File.ReadAllText(_filePath));
	}
	
	public void Save()
	{
		if(location == FileLocation.WWW)
		{
			Debug.LogError("Cant save file to remote");
			return;
		}
		
		File.WriteAllText(_filePath, JsonUtility.ToJson(Data));
	}
}

[System.Serializable]
public enum FileLocation
{
	LocalDirect = 0,
	ApplicationData = 1,
	WWW = 2
}

[System.Serializable]
public enum DataType
{
	Binary,
	JSON
}

[System.Serializable]
public class GameData
{
	public string[] judgeNames;
}