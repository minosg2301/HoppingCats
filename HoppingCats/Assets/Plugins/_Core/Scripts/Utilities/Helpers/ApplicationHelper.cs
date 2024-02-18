using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using moonNest;

public static class ApplicationHelper
{
    public static async UniTask<GeoCountry> GetGeoCountry()
    {
        try
        {
            return await GameServiceAPI.Get<GeoCountry>("/get/geo");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }
}

[Serializable]
public class GeoCountry
{
    public int[] range;
    public string country;
}