using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleSaveLoadSystem: MonoBehaviour, IDataPersistence
{
	//Sample script how to use save and load the data from the save system

	int scores;

	public void LoadData(GameData data)
	{
		scores = data.scores;
	}

	public void SaveData(GameData data)
	{
		data.scores = scores;
	}
}
