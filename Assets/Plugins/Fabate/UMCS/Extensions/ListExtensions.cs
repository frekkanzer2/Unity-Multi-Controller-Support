using System.Collections.Generic;

public static class ListExtensions
{
    public static bool IsEmpty<T>(this List<T> list) => list.Count == 0;
}
