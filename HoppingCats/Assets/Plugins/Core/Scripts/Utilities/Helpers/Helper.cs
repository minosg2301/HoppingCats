using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


public class Helper
{
    private static int Randx { get; set; }
    private static int Randy { get; set; }
    private static int Randz { get; set; }
    private static int Randw { get; set; }

    private static bool hasSetRandSeed;

    // @url http://answers.unity3d.com/questions/585035/lookat-2d-equivalent-.html
    public static Quaternion Rotate2D(Vector3 start, Vector3 end)
    {
        Vector3 diff = start - end;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, rot_z - 90f);
    }

    public static int GetARandomNumber()
    {
        if (!hasSetRandSeed)
        {
            Randx = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            Randy = UnityEngine.Random.Range(int.MinValue, 213214454);
            Randy = UnityEngine.Random.Range(int.MinValue, 131313212);
            Randz = UnityEngine.Random.Range(int.MinValue, 65014874);
            hasSetRandSeed = true;
        }
        int t = Randx ^ (Randx << 11);
        Randx = Randy; Randy = Randz; Randz = Randw;
        return Randw = Randw ^ (Randw >> 19) ^ t ^ (t >> 8);
    }

    public static string GenerateSHA256(string name)
    {
        Debug.Log("Generating SHA256 hash");
        StringBuilder sb = new StringBuilder();

        using (SHA256Managed sha256 = new SHA256Managed())
        {
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(name));
            foreach (byte b in hash)
                sb.Append(b.ToString("X2"));
        }
        return sb.ToString();
    }

    public static string ToHourFormat(double seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        if (timeSpan.Days > 0)
        {
            return timeSpan.ToString(@"d\d\ hh\:mm\:ss");
        }
        else
        {
            return seconds > 3600 ? timeSpan.ToString(@"hh\:mm\:ss") : timeSpan.ToString(@"mm\:ss");
        }
    }
}

