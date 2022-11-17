using System;
using System.Collections;
using System.Collections.Generic;

public static class Utilities
{
    public static T PickRandom<T>(this T[] array)
    {
        var r = new Random();
        var choice = r.Next(0, array.Length - 1);
        return array[choice];
    }

    public enum LevelType {
        obstacle,
        boss,
        multiplayer
    }
}
