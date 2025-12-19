using System.Runtime.InteropServices;
using CreativeCoders.Core;
using CreativeCoders.Core.IO;
using CreativeCoders.MacOS.Core.Foundation;

namespace CreativeCoders.MacOS.UserDefaults;

public class CoreFoundationPlistConverter : IPlistConverter
{
    private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

    // CFPropertyList format constants
    private const int XmlFormat = 100; // kCFPropertyListXMLFormat_v1_0
    private const int BinaryFormat = 200; // kCFPropertyListBinaryFormat_v1_0

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

    public Task<byte[]> ConvertBytesAsync(byte[] inputData, PlistFormat format)
    {
        if (!OperatingSystem.IsMacOS())
            throw new PlatformNotSupportedException("CFPlistConverter is supported only on macOS.");

        Ensure.NotNull(inputData);

        if (format == PlistFormat.Original)
        {
            return Task.FromResult(inputData);
        }

        var error = IntPtr.Zero;

        try
        {
            using var inDataRef = CFDataCreate(IntPtr.Zero, inputData, new IntPtr(inputData.Length))
                .ToCFIntPtr(message: "Failed to create CFData from input bytes.");

            using var plistRef = CFPropertyListCreateWithData(IntPtr.Zero, inDataRef, 0, out _, out error)
                .ToCFIntPtr(message: "Failed to parse plist input. The data is not a valid plist.");

            var desiredFormat = format == PlistFormat.Binary ? BinaryFormat : XmlFormat;
            using var outDataRef = CFPropertyListCreateData(IntPtr.Zero, plistRef, desiredFormat, 0, out error)
                .ToCFIntPtr(message: "Failed to serialize plist to desired format.");

            var ptr = CFDataGetBytePtr(outDataRef);
            var lenPtr = CFDataGetLength(outDataRef);
            var length = lenPtr.ToInt64();

            var result = new byte[length];
            Marshal.Copy(ptr, result, 0, (int)length);
            return Task.FromResult(result);
        }
        finally
        {
            if (error != IntPtr.Zero)
            {
                CFRelease(error);
            }
        }
    }

    public async Task ConvertFileAsync(string sourceFileName, string outputFileName, PlistFormat format)
    {
        if (!OperatingSystem.IsMacOS())
            throw new PlatformNotSupportedException("CFPlistConverter is supported only on macOS.");

        Ensure.FileExists(sourceFileName);

        if (format == PlistFormat.Original)
        {
            if (sourceFileName != outputFileName)
            {
                FileSys.File.Copy(sourceFileName, outputFileName);
            }

            return;
        }

        var inputBytes = await File.ReadAllBytesAsync(sourceFileName).ConfigureAwait(false);
        var converted = await ConvertBytesAsync(inputBytes, format).ConfigureAwait(false);

        await File.WriteAllBytesAsync(outputFileName, converted).ConfigureAwait(false);
    }
}
