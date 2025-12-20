namespace CreativeCoders.MacOS.Core.Foundation;

public class MacOSCoreFoundation : ICoreFoundation
{
    IntPtr ICoreFoundation.CFDataCreate(IntPtr allocator, byte[] bytes, IntPtr length)
    {
        return CoreFoundation.CFDataCreate(allocator, bytes, length);
    }

    IntPtr ICoreFoundation.CFDataGetBytePtr(IntPtr data)
    {
        return CoreFoundation.CFDataGetBytePtr(data);
    }

    IntPtr ICoreFoundation.CFDataGetLength(IntPtr data)
    {
        return CoreFoundation.CFDataGetLength(data);
    }

    IntPtr ICoreFoundation.CFPropertyListCreateWithData(IntPtr allocator, IntPtr data, uint options, out int format,
        out IntPtr error)
    {
        return CoreFoundation.CFPropertyListCreateWithData(allocator, data, options, out format, out error);
    }

    IntPtr ICoreFoundation.CFPropertyListCreateData(IntPtr allocator, IntPtr plist, int format, uint options,
        out IntPtr error)
    {
        return CoreFoundation.CFPropertyListCreateData(allocator, plist, format, options, out error);
    }

    void ICoreFoundation.CFRelease(IntPtr cf)
    {
        CoreFoundation.CFRelease(cf);
    }
}
