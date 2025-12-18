namespace CreativeCoders.MacOS.UserDefaults;

public interface IPlistConverter
{
    Task ConvertFileAsync(string sourceFileName, string outputFileName, PlistFormat format);

    Task<byte[]> ConvertBytesAsync(byte[] inputData, PlistFormat format);
}
