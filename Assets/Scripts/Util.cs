using System;
using System.Collections.Generic;

public static class Util
{
    public static void ForEach<T>(this IEnumerable<T> enumerator, Action<T> action)
    {
        foreach (T position in enumerator) action(position);
    }
}

// Fix https://stackoverflow.com/a/64749403
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}