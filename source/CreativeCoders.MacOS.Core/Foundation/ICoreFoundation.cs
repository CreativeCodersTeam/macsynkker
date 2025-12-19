using System.Runtime.InteropServices;

namespace CreativeCoders.MacOS.Core.Foundation;

public interface ICoreFoundation
{
    IntPtr CFDataCreate(IntPtr allocator, byte[] bytes, IntPtr length);

    IntPtr CFDataGetBytePtr(IntPtr data);

    IntPtr CFDataGetLength(IntPtr data);

    IntPtr CFPropertyListCreateWithData(IntPtr allocator, IntPtr data, uint options,
        out int format, out IntPtr error);

    IntPtr CFPropertyListCreateData(IntPtr allocator, IntPtr plist, int format, uint options,
        out IntPtr error);

    void CFRelease(IntPtr cf);
}

public class MacOSCoreFoundation : ICoreFoundation
{
    private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

    [DllImport(CoreFoundation)]
    private static extern IntPtr CFDataCreate(IntPtr allocator, [In] byte[] bytes, IntPtr length);

    [DllImport(CoreFoundation)]
    private static extern IntPtr CFDataGetBytePtr(IntPtr data);

    [DllImport(CoreFoundation)]
    private static extern IntPtr CFDataGetLength(IntPtr data);

    [DllImport(CoreFoundation)]
    private static extern IntPtr CFPropertyListCreateWithData(IntPtr allocator, IntPtr data, uint options,
        out int format, out IntPtr error);

    [DllImport(CoreFoundation)]
    private static extern IntPtr CFPropertyListCreateData(IntPtr allocator, IntPtr plist, int format, uint options,
        out IntPtr error);

    [DllImport(CoreFoundation)]
    private static extern void CFRelease(IntPtr cf);


    IntPtr ICoreFoundation.CFDataCreate(IntPtr allocator, byte[] bytes, IntPtr length)
    {
        return CFDataCreate(allocator, bytes, length);
    }

    IntPtr ICoreFoundation.CFDataGetBytePtr(IntPtr data)
    {
        return CFDataGetBytePtr(data);
    }

    IntPtr ICoreFoundation.CFDataGetLength(IntPtr data)
    {
        return CFDataGetLength(data);
    }

    IntPtr ICoreFoundation.CFPropertyListCreateWithData(IntPtr allocator, IntPtr data, uint options, out int format,
        out IntPtr error)
    {
        return CFPropertyListCreateWithData(allocator, data, options, out format, out error);
    }

    IntPtr ICoreFoundation.CFPropertyListCreateData(IntPtr allocator, IntPtr plist, int format, uint options,
        out IntPtr error)
    {
        return CFPropertyListCreateData(allocator, plist, format, options, out error);
    }

    void ICoreFoundation.CFRelease(IntPtr cf)
    {
        CFRelease(cf);
    }
}
