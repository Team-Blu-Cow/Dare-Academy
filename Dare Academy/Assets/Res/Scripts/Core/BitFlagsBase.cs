public abstract class BitFlagsBase
{
    public static int NumberOfFlags<T>() where T : System.Enum
    {
        return System.Enum.GetNames(typeof(T)).Length;
    }

    public static string[] FlagNames<T>() where T : System.Enum
    {
        return System.Enum.GetNames(typeof(T));
    }
}