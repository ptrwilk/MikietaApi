using System.Reflection;

namespace MikietaApi.Tests;

public static class Helpers
{
    public static TResult InvokePrivateMethod<TResult>(object obj, string methodName, params object[] args)
    {
        var method = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        return (TResult)method.Invoke(obj, args);
    }
}