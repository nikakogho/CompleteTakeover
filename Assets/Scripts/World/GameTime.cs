using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GameTime
{
    [Range(0, 99)] public int hours;
    [Range(0, 59)] public int minutes;
    [Range(0, 59)] public int seconds;

    public float TotalHours => hours + minutes / 60f + seconds / 3600f;
    public float TotalMinutes => hours * 60 + minutes + seconds / 60f;
    public float TotalSeconds => hours * 3600 + minutes * 60 + seconds;

    #region Constructors

    public GameTime(int hours, int minutes, int seconds)
    {
        this.hours = hours;
        this.minutes = minutes;
        this.seconds = seconds;
    }

    public GameTime(int totalSeconds)
    :this(totalSeconds / 3600, (totalSeconds % 3600) / 60, totalSeconds % 60)
    {

    }

    public GameTime(TimeSpan span)
    :this((int)span.TotalHours, span.Minutes, span.Seconds)
    {

    }

    #endregion

    #region Statics

    #region Standard

    public static GameTime Zero => new GameTime(0, 0, 0);
    public static GameTime Hour => new GameTime(1, 0, 0);
    public static GameTime Minute => new GameTime(0, 1, 0);
    public static GameTime Second => new GameTime(0, 0, 1);

    #endregion

    #region Special

    public static GameTime FreeRushTime => new GameTime(0, 5, 0);
    public static GameTime OneGemWorthTime => new GameTime(0, 40, 0);

    #endregion

    #endregion

    #region Properties

    public int GemCost => this <= FreeRushTime ? 0 : (1 + this / OneGemWorthTime);
    public bool Done => hours == 0 && minutes == 0 && seconds == 0;

    #endregion

    #region Operators

    #region Comparison

    public override bool Equals(object obj)
    {
        if (obj is GameTime) return this == (GameTime)obj;
        return false;
    }

    public override int GetHashCode()
    {
        return hours.GetHashCode() ^ minutes.GetHashCode() ^ seconds.GetHashCode();
    }

    public static bool operator==(GameTime a, GameTime b)
    {
        return a.hours == b.hours && a.minutes == b.minutes && a.seconds == b.seconds;
    }

    public static bool operator!=(GameTime a, GameTime b)
    {
        return !(a == b);
    }

    public static bool operator >(GameTime a, GameTime b)
    {
        if (a.hours > b.hours) return true;
        if (a.hours < b.hours) return false;
        if (a.minutes > b.minutes) return true;
        if (a.minutes < b.minutes) return false;
        return a.seconds > b.seconds;
    }

    public static bool operator <(GameTime a, GameTime b)
    {
        if (a.hours < b.hours) return true;
        if (a.hours > b.hours) return false;
        if (a.minutes < b.minutes) return true;
        if (a.minutes > b.minutes) return false;
        return a.seconds < b.seconds;
    }

    public static bool operator>=(GameTime a, GameTime b)
    {
        return a == b || a > b;
    }

    public static bool operator<=(GameTime a, GameTime b)
    {
        return a == b || a < b;
    }

    #endregion

    #region Arithmetics

    public static GameTime operator+(GameTime a, GameTime b)
    {
        int seconds = a.seconds + b.seconds;
        int extra = seconds >= 60 ? 1 : 0;
        seconds %= 60;
        int minutes = a.minutes + b.minutes + extra;
        extra = minutes >= 60 ? 1 : 0;
        minutes %= 60;
        int hours = a.hours + b.hours + extra;
        return new GameTime(hours, minutes, seconds);
    }

    public static GameTime operator -(GameTime a, GameTime b)
    {
        if (a < b) throw new System.ArgumentException("first parameter must be more than the second!");
        int seconds = a.seconds - b.seconds;
        int minutes = a.minutes - b.minutes;
        int hours = a.hours - b.hours;
        if (seconds < 0) { seconds += 60; minutes--; }
        if(minutes < 0) { minutes += 60; hours--; }
        return new GameTime(hours, minutes, seconds);
    }

    public static GameTime operator*(GameTime a, int b)
    {
        int seconds = a.seconds * b;
        int extra = seconds / 60;
        seconds %= 60;
        int minutes = a.minutes * b + extra;
        extra = minutes / 60;
        minutes %= 60;
        int hours = a.hours * b + extra;
        return new GameTime(hours, minutes, seconds);
    }

    public static int operator/(GameTime a, GameTime b)
    {
        float sa = a.TotalSeconds;
        float sb = b.TotalSeconds;
        int s = (int)(sa / sb);
        return s;
    }

    public static GameTime operator%(GameTime a, GameTime b)
    {
        while (a >= b) a -= b;
        return a;
    }

    #endregion

    #endregion
}
