using System.Runtime.InteropServices;

namespace CreativeCoders.MacOS.Core.Foundation;

public static class CoreFoundation
{
    private const string CoreFoundationLib = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

    [DllImport(CoreFoundationLib, EntryPoint = "CFDataCreate")]
    private static extern IntPtr CFDataCreateNative(IntPtr allocator, [In] byte[] bytes, IntPtr length);

    [DllImport(CoreFoundationLib, EntryPoint = "CFDataGetBytePtr")]
    private static extern IntPtr CFDataGetBytePtrNative(IntPtr data);

    [DllImport(CoreFoundationLib, EntryPoint = "CFDataGetLength")]
    private static extern IntPtr CFDataGetLengthNative(IntPtr data);

    [DllImport(CoreFoundationLib, EntryPoint = "CFPropertyListCreateWithData")]
    private static extern IntPtr CFPropertyListCreateWithDataNative(IntPtr allocator, IntPtr data, uint options,
        out int format, out IntPtr error);

    [DllImport(CoreFoundationLib, EntryPoint = "CFPropertyListCreateData")]
    private static extern IntPtr CFPropertyListCreateDataNative(IntPtr allocator, IntPtr plist, int format,
        uint options,
        out IntPtr error);

    [DllImport(CoreFoundationLib, EntryPoint = "CFRelease")]
    private static extern void CFReleaseNative(IntPtr cf);

    /// <summary>
    /// Creates an immutable CFData object from a buffer.
    /// </summary>
    /// <param name="allocator">The allocator to use to create the CFData object. Pass <see cref="IntPtr.Zero"/> to use the default allocator.</param>
    /// <param name="bytes">The buffer to use.</param>
    /// <param name="length">The number of bytes in <paramref name="bytes"/>.</param>
    /// <returns>A new CFData object, or <see cref="IntPtr.Zero"/> if a problem occurred.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="bytes"/> is null.</exception>
    public static IntPtr CFDataCreate(IntPtr allocator, byte[] bytes, IntPtr length)
    {
        return bytes is null
            ? throw new ArgumentNullException(nameof(bytes))
            : CFDataCreateNative(allocator, bytes, length);
    }

    /// <summary>
    /// Returns a pointer to the byte buffer of a CFData object.
    /// </summary>
    /// <param name="data">The CFData object.</param>
    /// <returns>A pointer to the byte buffer of <paramref name="data"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="data"/> is <see cref="IntPtr.Zero"/>.</exception>
    public static IntPtr CFDataGetBytePtr(IntPtr data)
    {
        return data == IntPtr.Zero
            ? throw new ArgumentException("Data pointer cannot be zero.", nameof(data))
            : CFDataGetBytePtrNative(data);
    }

    /// <summary>
    /// Returns the number of bytes in a CFData object.
    /// </summary>
    /// <param name="data">The CFData object.</param>
    /// <returns>The number of bytes in <paramref name="data"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="data"/> is <see cref="IntPtr.Zero"/>.</exception>
    public static IntPtr CFDataGetLength(IntPtr data)
    {
        return data == IntPtr.Zero
            ? throw new ArgumentException("Data pointer cannot be zero.", nameof(data))
            : CFDataGetLengthNative(data);
    }

    /// <summary>
    /// Creates a property list from XML or binary data.
    /// </summary>
    /// <param name="allocator">The allocator to use. Pass <see cref="IntPtr.Zero"/> to use the default allocator.</param>
    /// <param name="data">The data to parse.</param>
    /// <param name="options">Options for creating the property list.</param>
    /// <param name="format">On return, the format of the property list.</param>
    /// <param name="error">On return, an error object if an error occurred.</param>
    /// <returns>A property list object, or <see cref="IntPtr.Zero"/> if an error occurred.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="data"/> is <see cref="IntPtr.Zero"/>.</exception>
    public static IntPtr CFPropertyListCreateWithData(IntPtr allocator, IntPtr data, uint options,
        out int format, out IntPtr error)
    {
        return data == IntPtr.Zero
            ? throw new ArgumentException("Data pointer cannot be zero.", nameof(data))
            : CFPropertyListCreateWithDataNative(allocator, data, options, out format, out error);
    }

    /// <summary>
    /// Creates XML or binary data representing a property list.
    /// </summary>
    /// <param name="allocator">The allocator to use. Pass <see cref="IntPtr.Zero"/> to use the default allocator.</param>
    /// <param name="plist">The property list to serialize.</param>
    /// <param name="format">The format in which to serialize the property list.</param>
    /// <param name="options">Options for serializing the property list.</param>
    /// <param name="error">On return, an error object if an error occurred.</param>
    /// <returns>A CFData object containing the serialized property list, or <see cref="IntPtr.Zero"/> if an error occurred.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="plist"/> is <see cref="IntPtr.Zero"/>.</exception>
    public static IntPtr CFPropertyListCreateData(IntPtr allocator, IntPtr plist, int format, uint options,
        out IntPtr error)
    {
        return plist == IntPtr.Zero
            ? throw new ArgumentException("Property list pointer cannot be zero.", nameof(plist))
            : CFPropertyListCreateDataNative(allocator, plist, format, options, out error);
    }

    /// <summary>
    /// Releases a Core Foundation object.
    /// </summary>
    /// <param name="cf">The Core Foundation object to release.</param>
    public static void CFRelease(IntPtr cf)
    {
        if (cf == IntPtr.Zero)
        {
            return;
        }

        CFReleaseNative(cf);
    }
}
