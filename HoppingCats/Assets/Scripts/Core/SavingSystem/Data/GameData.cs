using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameData : MonoBehaviour
{
    //Declare data here
    #region declare
    public int scores;
    public SerializableDictionary<string, bool> scriptableObject;
    #endregion


    public GameData()
    {
        this.scores = 0;
        this.scriptableObject = new SerializableDictionary<string, bool>();
    }
}
