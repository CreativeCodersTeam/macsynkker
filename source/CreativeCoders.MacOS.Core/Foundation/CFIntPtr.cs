using System.Runtime.InteropServices;

namespace CreativeCoders.MacOS.Core.Foundation;

public sealed class CFIntPtr(IntPtr ptr) : IDisposable
{
    private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

    [DllImport(CoreFoundation)]
    private static extern void CFRelease(IntPtr cf);

    public IntPtr Ptr { get; } = ptr;

    public void Dispose()
    {
        if (Ptr != IntPtr.Zero)
        {
            CFRelease(Ptr);
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
