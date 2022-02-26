namespace SpinLockUtilNS;

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

    public static bool IsLockedOnce(ref int isLocked)
    {
        return IsStateExpected(
            currentStateRef: ref isLocked,
            expectedCurrentState: True
        );
    }

    public static bool IsStateExpected(
        ref int currentStateRef,
        int expectedCurrentState
    )
    {
        return ReadAtomically(currentStateRef: ref currentStateRef) == expectedCurrentState;
    }

    public static bool IsStateNotEqualTo(
        ref int currentStateRef,
        int stateItShouldNotEqualTo
    )
    {
        return ReadAtomically(currentStateRef: ref currentStateRef) != stateItShouldNotEqualTo;
    }

    public static bool IsUnlocked(ref int isLocked)
    {
        return IsStateExpected(
            currentStateRef: ref isLocked,
            expectedCurrentState: False
        );
    }

    /// <summary>
    ///     According to https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/variables#atomicity-of-variable-references
    ///     reads of `int`-s are atomic, but we want to be sure that no instruction reordering or cache happens,
    ///     hence we use <see cref="Thread.VolatileRead"/>
    /// </summary>
    public static int ReadAtomically(ref int currentStateRef)
    {
        return Thread.VolatileRead(address: ref currentStateRef);
    }

    public static bool TryLock(ref int isLocked)
    {
        return TrySetState(
            currentStateRef: ref isLocked,
            expectedCurrentState: False,
            newState: True
        );
    }

    public static bool TrySetState(
        ref int currentStateRef,
        int expectedCurrentState,
        int newState
    )
    {
        return Interlocked.CompareExchange(
            comparand: expectedCurrentState,
            location1: ref currentStateRef,
            value: newState
        ) == expectedCurrentState;
    }

    public static void UnlockOne(ref int isLocked)
    {
        Interlocked.Decrement(location: ref isLocked);
    }
}