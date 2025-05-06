using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    /// <summary>
    /// Invokes the provided function after the specified delay in seconds
    /// </summary>
    /// <param name="delay">The number of seconds to delay the function by</param>
    /// <param name="action">The funciton to run after the specified delay</param>
    public static void RunAfter(this MonoBehaviour behviour, float delay, System.Action action)
    {
        IEnumerator Delay()
        {
            yield return new WaitForSeconds(delay);
            action();
        }

        behviour.StartCoroutine(Delay());
    }

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

    public static Vector2 Reciprocal(this Vector2 vector)
    {
        return new Vector2(
            1 / vector.x,
            1 / vector.y
        );
    }

    public static Vector3 Reciprocal(this Vector3 vector)
    {
        return new Vector3(
            1 / vector.x,
            1 / vector.y,
            1 / vector.z
        );
    }
}

// Fix https://stackoverflow.com/a/64749403
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}