namespace SpinLockUtilNS;

/// <summary>
///     <see cref="SpinLockUtil"/> is needed when we know for sure that we don't want to block thread or run other
///     logic, which can change depending on implementation, we need just spin
/// </summary>
public static class SpinLockUtil
{
    public const int False = 0;
    public const int True = 1;

    public static void AcquireLock(this ref int isLocked)
    {
        while (true)
            if (TryLock(isLocked: ref isLocked))
                break;
    }

    /// <summary>
    ///     According to https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/variables#atomicity-of-variable-references
    ///     reads of `int`-s are atomic
    /// </summary>
    public static bool IsLockedOnce(this ref int isLocked)
    {
        return isLocked == True;
    }

    /// <summary>
    ///     According to https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/variables#atomicity-of-variable-references
    ///     reads of `int`-s are atomic
    /// </summary>
    public static bool IsUnlocked(this ref int isLocked)
    {
        return isLocked == False;
    }

    public static bool TryLock(this ref int isLocked)
    {
        return Interlocked.CompareExchange(
            location1: ref isLocked,
            value: True,
            comparand: False
        ) == False;
    }

    public static void UnlockOne(this ref int isLocked)
    {
        Interlocked.Decrement(location: ref isLocked);
    }
}