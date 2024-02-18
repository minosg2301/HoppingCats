using System;
using System.Reflection;

public class CSharpAssemblyHelper
{
    private static Assembly CSharpAssembly;

    public static Assembly GetAssemblyNameContainingType(string assemblyName)
    {
        foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if(assembly.FullName.Contains(assemblyName))
                return assembly;
        }

        return null;
    }

    public static T CreateInstance<T>(string className)
    {
        if(CSharpAssembly == null)
        {
            CSharpAssembly = GetAssemblyNameContainingType("Assembly-CSharp, Version=0.0.0.0");
        }
        return (T)CSharpAssembly.CreateInstance(className);
    }
}