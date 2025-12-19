namespace CreativeCoders.MacOS.Core.Foundation;

public static class IntPtrExtensions
{
    public static CFIntPtr ToCFIntPtr(this IntPtr ptr, bool throwIfZero = true, string message = "CFIntPtr is zero.")
    {
        var cfPtr = new CFIntPtr(ptr);

        if (throwIfZero)
        {
            cfPtr.ThrowIfZero(message);
        }

        return cfPtr;
    }
}
