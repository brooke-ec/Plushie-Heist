using System;
using System.Collections.Generic;

public static class Util
{
    public static void ForEach<T>(this IEnumerable<T> enumerator, Action<T> action)
    {
        foreach (T position in enumerator) action(position);
    }
}
