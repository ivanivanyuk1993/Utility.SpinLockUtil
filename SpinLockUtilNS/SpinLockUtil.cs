﻿namespace SpinLockUtilNS;

/// <summary>
///     <see cref="SpinLockUtil"/> is needed when we know for sure that we don't want to block thread or run other
///     logic, which can change depending on implementation, we need just spin
/// </summary>
public static class SpinLockUtil
{
    public const int False = 0;
    public const int True = 1;

    public static void AcquireLock(ref int isLocked)
    {
        while (true)
            if (TryLock(isLocked: ref isLocked))
                break;
    }

    /// <summary>
    ///     According to https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/variables#atomicity-of-variable-references
    ///     reads of `int`-s are atomic, but we want to be sure that no instruction reordering or cache happens,
    ///     hence we use <see cref="Thread.VolatileRead"/>
    /// </summary>
    public static bool IsLockedOnce(ref int isLocked)
    {
        return Thread.VolatileRead(address: ref isLocked) == True;
    }

    /// <summary>
    ///     According to https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/variables#atomicity-of-variable-references
    ///     reads of `int`-s are atomic, but we want to be sure that no instruction reordering or cache happens,
    ///     hence we use <see cref="Thread.VolatileRead"/>
    /// </summary>
    public static bool IsUnlocked(ref int isLocked)
    {
        return Thread.VolatileRead(address: ref isLocked) == False;
    }

    public static bool TryLock(ref int isLocked)
    {
        return Interlocked.CompareExchange(
            location1: ref isLocked,
            value: True,
            comparand: False
        ) == False;
    }

    public static void UnlockOne(ref int isLocked)
    {
        Interlocked.Decrement(location: ref isLocked);
    }
}