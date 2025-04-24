using System;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static void ForEach<T>(this IEnumerator<T> enumerator, Action<T> action)
    {
        using (enumerator) while (enumerator.MoveNext()) action(enumerator.Current);
    }

    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        ForEach(enumerable.GetEnumerator(), action);
    }

    public static Vector2Int Clamp(Vector2Int vector, Vector2Int min, Vector2Int max)
    {
        return Vector2Int.Min(Vector2Int.Max(vector, min), max);
    }
}

// Fix https://stackoverflow.com/a/64749403
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}