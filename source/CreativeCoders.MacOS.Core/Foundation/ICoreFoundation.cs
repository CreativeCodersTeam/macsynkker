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
