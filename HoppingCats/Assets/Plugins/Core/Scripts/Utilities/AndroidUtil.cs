#if UNITY_ANDROID
using System.Linq;
using UnityEngine;

public static class AndroidUtil
{
    public static void CallJavaStaticMethod(string className, string method, params object[] args)
    {
        using AndroidJavaClass jObj = new AndroidJavaClass(className);
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].GetType() == typeof(string[]))
            {
                args[i] = JavaArrayFromCS(args[i] as string[]);
            }
        }
        jObj.CallStatic(method, args);
    }

    public static T CallJavaStaticMethod<T>(string className, string method, params object[] args)
    {
        using AndroidJavaClass jObj = new AndroidJavaClass(className);
        return jObj.CallStatic<T>(method, args);
    }

    private static AndroidJavaObject JavaArrayFromCS(string[] values)
    {
        AndroidJavaClass arrayClass = new AndroidJavaClass("java.lang.reflect.Array");
        AndroidJavaObject arrayObject = arrayClass.CallStatic<AndroidJavaObject>("newInstance", new AndroidJavaClass("java.lang.String"), values.Count());
        for (int i = 0; i < values.Count(); ++i)
        {
            arrayClass.CallStatic("set", arrayObject, i, new AndroidJavaObject("java.lang.String", values[i]));
        }

        return arrayObject;
    }
}
#endif