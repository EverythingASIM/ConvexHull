using System.Collections.Generic;

public static class ListExtensions
{
    //====================================================================================================
    //====================================================================================================

    /// <summary>
    /// Similar to functionality to stack.pop - remove and return last recently added entry
    /// </summary>
    public static T Pop<T>(this List<T> list)
    {
        var index = list.Count - 1;
        T temp = list[index];
        list.RemoveAt(index);
        return temp;
    }

    /// <summary>
    /// Remove null from list
    /// </summary>
    public static List<T> RemoveNulls<T>(this List<T> list)
    {
        for (var i = list.Count - 1; i > -1; i--)
        {
            if (list[i] == null) list.RemoveAt(i);
        }
        return list;
    }

    //====================================================================================================
    //====================================================================================================


    /// <summary>
    /// Remove items between two numbers
    /// Starting from Left To Right Will loop round to 0
    /// </summary>
    public static List<T> RemoveBetween<T>(this List<T> list, int left, int right)
    {
        if (left + 1 < right)
        {
            int startIndex = left + 1;
            list.RemoveRange(startIndex, right - startIndex);
        }
        else if (right < left)
        {
            //Remove Left to end of list
            int removeCount = list.Count - 1 - left;
            if (removeCount > 0) list.RemoveRange(left + 1, removeCount);

            //Remove start of list to Right
            list.RemoveRange(0, right);
        }

        return list;
    }

    /// <summary>
    /// Replaces the items between two numbers, with an item
    /// Starting from Left To Right Will loop round to 0
    /// </summary>
    public static List<T> ReplaceBetween<T>(this List<T> list, int left, int right, T replaceWithItem)
    {
        if (left + 1 < right)
        {
            int startIndex = left + 1;
            list.RemoveRange(startIndex, right - startIndex);

            list.Insert(startIndex, replaceWithItem);
        }
        else if (right < left)
        {
            //Remove Left to end of list
            int removeCount = list.Count - 1 - left;
            if (removeCount > 0) list.RemoveRange(left + 1, removeCount);

            //Remove start of list to Right
            list.RemoveRange(0, right);

            list.Add(replaceWithItem);
        }

        return list;
    }

    /// <summary>
    /// Replaces the items between two numbers, with an item
    /// Adds item if nothing is between
    /// Starting from Left To Right Will loop round to 0
    /// </summary>
    public static void AddReplaceBetween<T>(this List<T> list, int left, int right, T replaceWithItem)
    {
        if (left + 1 < right)
        {
            int startIndex = left + 1;
            list.RemoveRange(startIndex, right - startIndex);

            list.Insert(startIndex, replaceWithItem);
        }
        else if (right < left)
        {
            //Remove Left to end of list
            int removeCount = list.Count - 1 - left;
            if (removeCount > 0) list.RemoveRange(left + 1, removeCount);

            //Remove start of list to Right
            list.RemoveRange(0, right);

            list.Add(replaceWithItem);
        }
        else
        {
            list.Insert(right, replaceWithItem);
        }
    }
}
