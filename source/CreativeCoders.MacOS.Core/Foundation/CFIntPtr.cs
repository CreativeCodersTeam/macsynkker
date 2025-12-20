using System.Runtime.InteropServices;

namespace CreativeCoders.MacOS.Core.Foundation;

public sealed class CFIntPtr(IntPtr ptr) : IDisposable
{
    public IntPtr Ptr { get; } = ptr;

    public void Dispose()
    {
        if (Ptr != IntPtr.Zero)
        {
            CoreFoundation.CFRelease(Ptr);
        }
    }

    public void ThrowIfZero(string message = "CFIntPtr is zero.")
    {
        if (Ptr == IntPtr.Zero)
        {
            throw new InvalidOperationException(message);
        }
    }

    public static implicit operator IntPtr(CFIntPtr cfIntPtr) => cfIntPtr.Ptr;
}
